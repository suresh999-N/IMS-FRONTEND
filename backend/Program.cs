using System.Text;
using System.Data.Common;
using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using IMSBackend.Middleware;
using IMSBackend.Services;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Events;

using IMSBackend.Interfaces;
using IMSBackend.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// SERILOG
// =========================================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();


// =========================================================
// DATABASE
// =========================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is missing.");
}


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))
    )
);


// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => item.Key,
                item => item.Value!
                    .Errors
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(
            ApiResponse<object>.Fail(
                "Validation failed.",
                errors,
                context.HttpContext.TraceIdentifier));
    };
});


// =========================================================
// SWAGGER
// =========================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});


// =========================================================
// CORS
// =========================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredOrigins", policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        if (origins.Length == 0 &&
            builder.Environment.IsDevelopment())
        {
            origins =
            [
                "http://localhost:5173",
                "https://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:5174",
                "http://127.0.0.1:5174",
                "http://localhost:5175",
                "http://127.0.0.1:5175",
                "http://localhost:5176",
                "http://127.0.0.1:5176",
                "http://localhost:5273",
                "https://scarcity-array-dispense.ngrok-free.dev"
            ];
        }

        if (origins.Length == 0)
        {
            throw new InvalidOperationException(
                "Cors:AllowedOrigins must be configured outside Development.");
        }

        policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetPreflightMaxAge(TimeSpan.FromHours(1));
    });
});


// =========================================================
// JWT
// =========================================================

var jwtSettings =
    builder.Configuration.GetSection("Jwt");

var keyString = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(keyString))
{
    throw new InvalidOperationException(
        "Jwt:Key is missing.");
}

if (Encoding.UTF8.GetByteCount(keyString) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must be at least 32 bytes.");
}

var key = Encoding.UTF8.GetBytes(keyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger =
                context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

            logger.LogError(
                context.Exception,
                "JWT Authentication Failed. Path: {Path}",
                context.Request.Path);

            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            var logger =
                context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

            logger.LogWarning(
                "JWT Challenge. Path: {Path}. Error: {Error}",
                context.Request.Path,
                context.Error);

            return Task.CompletedTask;
        },

        OnTokenValidated = async context =>
        {
            var logger =
                context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

            var dbContext =
                context.HttpContext.RequestServices
                    .GetRequiredService<AppDbContext>();

            var userIdClaim =
                context.Principal?.FindFirst("UserId")?.Value;

            var tokenVersionClaim =
                context.Principal?.FindFirst("TokenVersion")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                context.Fail("Invalid UserId.");
                return;
            }

            if (!int.TryParse(
                    tokenVersionClaim,
                    out var tokenVersion))
            {
                context.Fail("Invalid TokenVersion.");
                return;
            }

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                context.Fail("User not found.");
                return;
            }

            if (user.TokenVersion != tokenVersion)
            {
                context.Fail(
                    "This session has expired. Please login again.");
                return;
            }

            logger.LogInformation(
                "JWT Valid. User: {User}",
                context.Principal?.Identity?.Name);
        }
    };

    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(key),

            ClockSkew = TimeSpan.Zero
        };
});


// =========================================================
// AUTHORIZATION
// =========================================================

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});


// =========================================================
// SERVICES
// =========================================================

builder.Services.AddScoped<PermissionService>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<
    IRefreshTokenService,
    RefreshTokenService>();

builder.Services.AddScoped<
    ILoginHistoryService,
    LoginHistoryService>();

builder.Services.AddHostedService<
    RefreshTokenCleanupService>();


// =========================================================
// PDF SERVICE
// =========================================================

builder.Services.AddScoped<PdfService>();


// =========================================================
// EMAIL SERVICE
// =========================================================

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<AuditLogService>();

builder.Services.AddScoped<InventoryHealthService>();


// =========================================================
// BUILD
// =========================================================

var app = builder.Build();


// =========================================================
// STARTUP DATABASE / PERMISSION SYNCHRONIZATION
// =========================================================

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        var connection =
            dbContext.Database.GetDbConnection();

        if (connection.State !=
            System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // =====================================================
        // EXISTING STARTUP SCHEMA SAFETY CHECKS
        // =====================================================

        await EnsureColumnExistsAsync(
            connection,
            "supplier_documents",
            "is_temporary",
            "BOOLEAN NOT NULL DEFAULT FALSE");

        await EnsureColumnExistsAsync(
            connection,
            "products",
            "is_archived",
            "BOOLEAN NOT NULL DEFAULT FALSE");

        await EnsureColumnExistsAsync(
            connection,
            "customer_payments",
            "is_cancelled",
            "BOOLEAN NOT NULL DEFAULT FALSE");

        await EnsureColumnExistsAsync(
            connection,
            "customer_payments",
            "cancelled_at",
            "DATETIME NULL");

        await EnsureColumnExistsAsync(
            connection,
            "customer_payments",
            "cancellation_reason",
            "TEXT NULL");

        await EnsureColumnExistsAsync(
            connection,
            "supplier_payments",
            "is_cancelled",
            "BOOLEAN NOT NULL DEFAULT FALSE");

        await EnsureColumnExistsAsync(
            connection,
            "supplier_payments",
            "cancelled_at",
            "DATETIME NULL");

        await EnsureColumnExistsAsync(
            connection,
            "supplier_payments",
            "cancellation_reason",
            "TEXT NULL");

        await EnsureColumnExistsAsync(
            connection,
            "categories",
            "status",
            "VARCHAR(32) NOT NULL DEFAULT 'Active'");


        // =====================================================
        // PUTAWAY AUDITS TABLE
        // =====================================================

        await EnsureTableExistsAsync(
            connection,
            "putaway_audits",
            @"
            CREATE TABLE `putaway_audits` (
                `putaway_audit_id` INT AUTO_INCREMENT PRIMARY KEY,
                `product_id` INT NOT NULL,
                `variant_id` INT NULL,
                `warehouse_id` INT NOT NULL,
                `rack_id` INT NOT NULL,
                `bin_id` INT NOT NULL,
                `quantity` DECIMAL(18, 2) NOT NULL,
                `user_id` INT NULL,
                `user_name` VARCHAR(256) NULL,
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

                INDEX `idx_putaway_audits_product_warehouse`
                    (`product_id`, `warehouse_id`),

                INDEX `idx_putaway_audits_bin`
                    (`bin_id`),

                INDEX `idx_putaway_audits_created_at`
                    (`created_at`)
            )");


        // =====================================================
        // BIN TRANSFER AUDITS TABLE
        // =====================================================

        await EnsureTableExistsAsync(
            connection,
            "bin_transfer_audits",
            @"
            CREATE TABLE `bin_transfer_audits` (
                `bin_transfer_audit_id` INT AUTO_INCREMENT PRIMARY KEY,
                `product_id` INT NOT NULL,
                `variant_id` INT NULL,
                `warehouse_id` INT NOT NULL,
                `from_bin_id` INT NOT NULL,
                `to_bin_id` INT NOT NULL,
                `quantity` DECIMAL(18, 2) NOT NULL,
                `user_id` INT NULL,
                `user_name` VARCHAR(256) NULL,
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

                INDEX `idx_bin_transfer_audits_product_warehouse`
                    (`product_id`, `warehouse_id`),

                INDEX `idx_bin_transfer_audits_from_bin`
                    (`from_bin_id`),

                INDEX `idx_bin_transfer_audits_to_bin`
                    (`to_bin_id`),

                INDEX `idx_bin_transfer_audits_created_at`
                    (`created_at`)
            )");


        // =====================================================
        // WAREHOUSE TRANSFER AUDITS TABLE
        // =====================================================

        await EnsureTableExistsAsync(
            connection,
            "warehouse_transfer_audits",
            @"
            CREATE TABLE `warehouse_transfer_audits` (
                `warehouse_transfer_audit_id`
                    INT AUTO_INCREMENT PRIMARY KEY,

                `transfer_id` INT NOT NULL,
                `product_id` INT NOT NULL,
                `variant_id` INT NULL,
                `from_warehouse_id` INT NOT NULL,
                `to_warehouse_id` INT NOT NULL,
                `quantity` DECIMAL(18, 2) NOT NULL,
                `user_id` INT NULL,
                `user_name` VARCHAR(256) NULL,
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

                INDEX `idx_warehouse_transfer_audits_transfer`
                    (`transfer_id`),

                INDEX `idx_warehouse_transfer_audits_product`
                    (`product_id`),

                INDEX `idx_warehouse_transfer_audits_from_warehouse`
                    (`from_warehouse_id`),

                INDEX `idx_warehouse_transfer_audits_to_warehouse`
                    (`to_warehouse_id`),

                INDEX `idx_warehouse_transfer_audits_created_at`
                    (`created_at`)
            )");


        // =====================================================
        // NEW:
        // ENSURE ALL ACTIVE ROLES HAVE ALL ACTIVE MODULE
        // PERMISSION RECORDS
        // =====================================================

        var permissionService =
            scope.ServiceProvider
                .GetRequiredService<PermissionService>();

        await permissionService
            .EnsurePermissionsForAllRolesAsync();

        app.Logger.LogInformation(
            "Role permission synchronization completed successfully.");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(
            ex,
            "Startup schema or permission synchronization failed.");
    }


    // =========================================================
    // CLEAN STALE TEMPORARY SUPPLIER DOCUMENTS
    // =========================================================

    try
    {
        var cutoff =
            DateTime.UtcNow.AddHours(-24);

        var staleTempDocuments =
            await dbContext.SupplierDocuments
                .Where(document =>
                    document.IsTemporary &&
                    document.UploadedAt < cutoff)
                .ToListAsync();

        foreach (var document in staleTempDocuments)
        {
            var fullPath =
                Path.Combine(
                    app.Environment.ContentRootPath,
                    "wwwroot",
                    string.IsNullOrWhiteSpace(
                        document.FilePath)
                        ? string.Empty
                        : document.FilePath.TrimStart('/'));

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(
                    ex,
                    "Could not delete stale staged supplier " +
                    "document file. DocumentId={DocumentId}, " +
                    "Path={Path}",
                    document.DocumentId,
                    fullPath);
            }
        }

        if (staleTempDocuments.Count > 0)
        {
            dbContext.SupplierDocuments
                .RemoveRange(staleTempDocuments);

            await dbContext.SaveChangesAsync();

            app.Logger.LogInformation(
                "Cleaned stale staged supplier documents. " +
                "Count={Count}",
                staleTempDocuments.Count);
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(
            ex,
            "Supplier document temp cleanup failed during startup.");
    }
}


// =========================================================
// MIDDLEWARE
// =========================================================

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMiddleware<SecurityHeadersMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                Path.Combine(
                    app.Environment.WebRootPath!,
                    "uploads")),

        RequestPath = "/uploads"
    });


app.UseRouting();

app.UseCors("ConfiguredOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


// =========================================================
// ENSURE COLUMN EXISTS
// =========================================================

static async Task EnsureColumnExistsAsync(
    DbConnection connection,
    string tableName,
    string columnName,
    string columnDefinition)
{
    await using var columnCheckCommand =
        connection.CreateCommand();

    columnCheckCommand.CommandText = @"
        SELECT COUNT(*)
        FROM information_schema.columns
        WHERE table_schema = DATABASE()
          AND table_name = @tableName
          AND column_name = @columnName";

    var tableParameter =
        columnCheckCommand.CreateParameter();

    tableParameter.ParameterName =
        "@tableName";

    tableParameter.Value =
        tableName;

    columnCheckCommand.Parameters.Add(
        tableParameter);


    var columnParameter =
        columnCheckCommand.CreateParameter();

    columnParameter.ParameterName =
        "@columnName";

    columnParameter.Value =
        columnName;

    columnCheckCommand.Parameters.Add(
        columnParameter);


    var columnExists =
        Convert.ToInt32(
            await columnCheckCommand
                .ExecuteScalarAsync()) > 0;

    if (columnExists)
    {
        return;
    }


    await using var alterCommand =
        connection.CreateCommand();

    alterCommand.CommandText =
        $"ALTER TABLE `{tableName}` " +
        $"ADD COLUMN `{columnName}` " +
        $"{columnDefinition}";

    await alterCommand.ExecuteNonQueryAsync();
}


// =========================================================
// ENSURE TABLE EXISTS
// =========================================================

static async Task EnsureTableExistsAsync(
    DbConnection connection,
    string tableName,
    string createTableSql)
{
    await using var tableCheckCommand =
        connection.CreateCommand();

    tableCheckCommand.CommandText = @"
        SELECT COUNT(*)
        FROM information_schema.tables
        WHERE table_schema = DATABASE()
          AND table_name = @tableName";

    var tableParameter =
        tableCheckCommand.CreateParameter();

    tableParameter.ParameterName =
        "@tableName";

    tableParameter.Value =
        tableName;

    tableCheckCommand.Parameters.Add(
        tableParameter);


    var tableExists =
        Convert.ToInt32(
            await tableCheckCommand
                .ExecuteScalarAsync()) > 0;

    if (tableExists)
    {
        return;
    }


    await using var createCommand =
        connection.CreateCommand();

    createCommand.CommandText =
        createTableSql;

    await createCommand.ExecuteNonQueryAsync();
}