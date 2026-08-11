import { useCallback, useEffect, useMemo, useState } from "react";
import { apiRequest, getResponseList } from "../../api/apiClient";
import { API_ENDPOINTS } from "../../api/endpoints";
import { showToast } from "../../components/common/toast";
import NotificationsHeader from "./components/NotificationsHeader";
import NotificationsTable from "./components/NotificationsTable";
import "./Notifications.css";

export default function Notifications() {
  const [notifications, setNotifications] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchNotifications = useCallback(async () => {
    setIsLoading(true);
    try {
      const response = await apiRequest(API_ENDPOINTS.notifications.list);
      const data = getResponseList(response);
      setNotifications(data || []);
    } catch (error) {
      console.error("Error fetching notifications:", error);
      showToast("Failed to load notifications.", "error");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchNotifications();
  }, [fetchNotifications]);

  const handleMarkRead = useCallback(async (id) => {
    try {
      await apiRequest(API_ENDPOINTS.notifications.read(id), { method: "PUT" });
      showToast("Notification marked as read.", "success");
      setNotifications((prev) =>
        prev.map((n) =>
          n.notificationId === id || n.id === id ? { ...n, isRead: true } : n,
        ),
      );
    } catch (error) {
      console.error("Error marking notification as read:", error);
      showToast("Failed to mark notification as read.", "error");
    }
  }, []);

  const summary = useMemo(() => {
    const total = notifications.length;
    const unread = notifications.filter((n) => !n.isRead).length;
    const critical = notifications.filter(
      (n) => String(n.type || "").toLowerCase() === "critical",
    ).length;
    const read = Math.max(total - unread, 0);

    return { total, unread, read, critical };
  }, [notifications]);

  return (
    <div className="page notifications-page">
      <NotificationsHeader summary={summary} />
      <NotificationsTable
        notifications={notifications}
        loading={isLoading}
        onMarkRead={handleMarkRead}
      />
    </div>
  );
}
