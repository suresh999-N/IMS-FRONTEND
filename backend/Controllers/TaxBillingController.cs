using IMSBackend.Data;
using IMSBackend.DTOs.SystemSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/tax-billing")]
    [Tags("SystemSettingsSection")]
    public class TaxBillingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const string SectionKey = "tax_billing";

        public TaxBillingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tax-billing
        [HttpGet]
        public async Task<IActionResult> GetTaxBillingRules()
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
            {
                return NotFound("Tax & Billing section not found.");
            }

            return Ok(section);
        }

        // PUT: api/tax-billing
        [HttpPut]
        public async Task<IActionResult> UpdateTaxBillingRules([FromBody] List<UpdateSystemRuleDto> rules)
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
                Message = "Tax & Billing rules updated successfully."
            });
        }

        // POST: api/tax-billing/reset
        [HttpPost("reset")]
        public async Task<IActionResult> ResetTaxBillingRules()
        {
            var section = await _context.SystemSettingSections
                .Include(x => x.Rules)
                .FirstOrDefaultAsync(x => x.SectionKey == SectionKey);

            if (section == null)
            {
                return NotFound("Tax & Billing section not found.");
            }

            foreach (var rule in section.Rules)
            {
                rule.RuleValue = rule.DefaultValue;
                rule.IsEnabled = rule.DefaultValue == "true";
                rule.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Tax & Billing rules reset successfully."
            });
        }
    }
}