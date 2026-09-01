using IMSBackend.Data;
using IMSBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL NOTIFICATIONS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _context.Notifications
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        // =========================
        // GET UNREAD COUNT
        // =========================
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _context.Notifications
                .CountAsync(x => !x.IsRead);

            return Ok(new
            {
                unreadCount = count
            });
        }

        // =========================
        // CREATE NOTIFICATION
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateNotification(
            Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Notification created successfully"
            });
        }

        // =========================
        // MARK AS READ
        // =========================
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification =
                await _context.Notifications
                    .FirstOrDefaultAsync(x =>
                        x.NotificationId == id);

            if (notification == null)
            {
                return NotFound(new
                {
                    message = "Notification not found"
                });
            }

            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Notification marked as read"
            });
        }

        // =========================
        // DELETE NOTIFICATION
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(
            int id)
        {
            var notification =
                await _context.Notifications
                    .FirstOrDefaultAsync(x =>
                        x.NotificationId == id);

            if (notification == null)
            {
                return NotFound(new
                {
                    message = "Notification not found"
                });
            }

            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Notification deleted successfully"
            });
        }
    }
}