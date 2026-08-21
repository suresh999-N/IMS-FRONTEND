import { API_ENDPOINTS } from './endpoints'
import { apiRequest } from './apiClient'

export async function getPurchaseReturns(query = {}) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.list, { query })
}

export async function getPurchaseReturnById(id) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.byId(id))
}

export async function getPurchaseReturnSuppliers() {
  return apiRequest(API_ENDPOINTS.purchaseReturns.suppliers)
}

export async function getPurchaseReturnGoodsReceipts(supplierId) {
  const query = supplierId ? { supplierId } : {}
  return apiRequest(API_ENDPOINTS.purchaseReturns.grns, { query })
}

export async function getGoodsReceiptReturnItems(grnId) {
  if (!grnId) return { success: true, data: [] }
  return apiRequest(API_ENDPOINTS.purchaseReturns.grnItems(grnId))
}

export async function createPurchaseReturn(payload) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.create, {
    method: 'POST',
    body: payload,
  })
}

export async function updatePurchaseReturn(id, payload) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.update(id), {
    method: 'PUT',
    body: payload,
  })
}

export async function deletePurchaseReturn(id) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.delete(id), {
    method: 'DELETE',
  })
}

export async function submitPurchaseReturn(id) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.submit(id), {
    method: 'POST',
  })
}

export async function approvePurchaseReturn(id) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.approve(id), {
    method: 'POST',
  })
}

export async function rejectPurchaseReturn(id, payload = {}) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.reject(id), {
    method: 'POST',
    body: payload,
  })
}

export async function completePurchaseReturn(id) {
  return apiRequest(API_ENDPOINTS.purchaseReturns.complete(id), {
    method: 'POST',
  })
}

export function getPurchaseReturnErrorMessage(response, fallback = 'Operation failed.') {
  if (!response) return fallback
  if (typeof response === 'string' && response.trim()) return response
  if (response?.message && typeof response.message === 'string' && response.message.trim()) return response.message
  if (response?.error && typeof response.error === 'string' && response.error.trim()) return response.error
  if (response?.data?.message && typeof response.data.message === 'string' && response.data.message.trim()) return response.data.message
  if (response?.errors && typeof response.errors === 'object') {
    const messages = Object.values(response.errors).flat().filter(Boolean)
    if (messages.length > 0) return messages.join(' ')
  }
  return fallback
}
