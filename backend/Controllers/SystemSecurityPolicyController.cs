using IMSBackend.Data;
using IMSBackend.DTOs.SystemSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/system-security-policy")]
    [Tags("SystemSettingsSection")]
    public class SystemSecurityPolicyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const string SectionKey = "system_security_policy";

        public SystemSecurityPolicyController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/system-security-policy
        [HttpGet]
        public async Task<IActionResult> GetSystemSecurityPolicyRules()
        {
            var section = await _context.SystemSettingSections
                .Include(x => x.Rules)
                .Where(x => x.SectionKey == SectionKey && x.IsActive)
                .Select(x => new
                {
                    x.SectionId,
                    x.SectionKey,
                    x.SectionName,
                    RuleCount = x.Rules.Count,
                    EnabledCount = x.Rules.Count(r => r.IsEnabled),
                    Rules = x.Rules
                        .OrderBy(r => r.DisplayOrder)
                        .Select(r => new
                        {
                            r.RuleId,
                            r.RuleKey,
                            r.RuleName,
                            r.RuleDescription,
                            r.RuleType,
                            r.RuleValue,
                            r.DefaultValue,
                            r.IsEnabled,
                            r.DisplayOrder
                        })
                })
                .FirstOrDefaultAsync();

            if (section == null)
                return NotFound("System Security Policy section not found.");

            return Ok(section);
        }

        // PUT: api/system-security-policy
        [HttpPut]
        public async Task<IActionResult> UpdateSystemSecurityPolicyRules([FromBody] List<UpdateSystemRuleDto> rules)
        {
            var dbRules = await _context.SystemSettingRules
                .Include(x => x.Section)
                .Where(x => x.Section!.SectionKey == SectionKey)
                .ToListAsync();

            foreach (var item in rules)
            {
                var rule = dbRules.FirstOrDefault(x => x.RuleId == item.RuleId);

                if (rule != null)
                {
                    rule.RuleValue = item.RuleValue;
                    rule.IsEnabled = item.IsEnabled;
                    rule.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "System Security Policy rules updated successfully."
            });
        }

        // POST: api/system-security-policy/reset
        [HttpPost("reset")]
        public async Task<IActionResult> ResetSystemSecurityPolicyRules()
        {
            var section = await _context.SystemSettingSections
                .Include(x => x.Rules)
                .FirstOrDefaultAsync(x => x.SectionKey == SectionKey);

            if (section == null)
                return NotFound("System Security Policy section not found.");

            foreach (var rule in section.Rules)
            {
                rule.RuleValue = rule.DefaultValue;
                rule.IsEnabled = rule.DefaultValue == "true";
                rule.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "System Security Policy rules reset successfully."
            });
        }
    }
}