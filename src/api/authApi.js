import { apiRequest } from "./apiClient";
import { API_ENDPOINTS } from "./endpoints";

const emailVerificationRequests = new Map();

function requireText(value, message) {
  const normalizedValue = String(value ?? "").trim();

  if (!normalizedValue) {
    throw new Error(message);
  }

  return normalizedValue;
}

function requireUserId(userId) {
  const normalizedUserId = Number(userId);

  if (!Number.isInteger(normalizedUserId) || normalizedUserId <= 0) {
    throw new Error("The signed-in user could not be identified.");
  }

  return normalizedUserId;
}

/**
 * 1. POST /api/auth/register
 */
export function registerAccount(userData) {
  return apiRequest(API_ENDPOINTS.auth.register, {
    method: "POST",
    body: userData,
  });
}

/**
 * 2. POST /api/auth/login
 */
export function loginUser(credentials) {
  return apiRequest(API_ENDPOINTS.auth.login, {
    method: "POST",
    body: credentials,
  });
}

/**
 * 3. POST /api/auth/refresh-token
 */
export function refreshAuthToken(refreshToken) {
  return apiRequest(API_ENDPOINTS.auth.refreshToken, {
    method: "POST",
    body: {
      refreshToken: requireText(refreshToken, "Refresh token is required."),
    },
  });
}

/**
 * 4. POST /api/auth/forgot-password
 */
export function requestForgotPassword(email) {
  return apiRequest(API_ENDPOINTS.auth.forgotPassword, {
    method: "POST",
    body: {
      email: requireText(email, "The email address is missing."),
    },
  });
}

/**
 * 5. PUT /api/auth/change-password/{userId}
 */
export function changePassword(userId, currentPassword, newPassword) {
  return apiRequest(API_ENDPOINTS.auth.changePassword(requireUserId(userId)), {
    method: "PUT",
    body: {
      currentPassword: requireText(currentPassword, "Current password is required."),
      newPassword: requireText(newPassword, "New password is required."),
    },
  });
}

/**
 * 6. GET /api/auth/verify-email
 */
export function verifyEmailAddress(token) {
  const normalizedToken = requireText(
    token,
    "The verification token is missing.",
  );
  const existingRequest = emailVerificationRequests.get(normalizedToken);

  if (existingRequest) {
    return existingRequest;
  }

  const request = apiRequest(API_ENDPOINTS.auth.verifyEmail, {
    method: "GET",
    query: {
      token: normalizedToken,
    },
  }).then((response) => {
    if (response.status === 0 || response.status >= 500) {
      emailVerificationRequests.delete(normalizedToken);
    }

    return response;
  });

  emailVerificationRequests.set(normalizedToken, request);
  return request;
}

/**
 * 7. POST /api/auth/logout/{userId}
 */
export function logoutCurrentSession(userId) {
  return apiRequest(API_ENDPOINTS.auth.logout(requireUserId(userId)), {
    method: "POST",
  });
}

/**
 * 8. POST /api/auth/logout-all-devices/{userId}
 */
export function logoutAllDevices(userId) {
  return apiRequest(
    API_ENDPOINTS.auth.logoutAllDevices(requireUserId(userId)),
    {
      method: "POST",
    },
  );
}

/**
 * 9. POST /api/auth/reset-password
 */
export function resetPassword(resetData) {
  return apiRequest(API_ENDPOINTS.auth.resetPassword, {
    method: "POST",
    body: resetData,
  });
}

/**
 * 10. GET /api/auth/claims
 */
export function getAuthClaims() {
  return apiRequest(API_ENDPOINTS.auth.claims, {
    method: "GET",
  });
}

/**
 * 11. POST /api/auth/verify-otp
 */
export function verifyOtp(email, otp) {
  return apiRequest(API_ENDPOINTS.auth.verifyOtp, {
    method: "POST",
    body: {
      email: requireText(email, "The email address is missing."),
      otp: requireText(otp, "The OTP code is missing."),
    },
  });
}

/**
 * 12. POST /api/auth/resend-login-otp
 */
export function resendLoginOtp(email) {
  return apiRequest(API_ENDPOINTS.auth.resendLoginOtp, {
    method: "POST",
    body: {
      email: requireText(email, "The email address is missing."),
    },
  });
}

/**
 * 13. POST /api/auth/resend-verification
 */
export function resendVerificationEmail(email, options = {}) {
  return apiRequest(API_ENDPOINTS.auth.resendVerification, {
    method: "POST",
    body: {
      email: requireText(email, "The email address is missing."),
    },
    signal: options.signal,
  });
}

export function validateCurrentSession() {
  return apiRequest(API_ENDPOINTS.auth.session, {
    method: "GET",
    timeoutMs: 10000,
  });
}
