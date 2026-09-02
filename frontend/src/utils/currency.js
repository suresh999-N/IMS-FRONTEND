export function formatINR(amount, { convertFromUSD = false, rate = 83 } = {}) {
  const numericValue = Number(amount) || 0
  const value = convertFromUSD ? numericValue * rate : numericValue

  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value)
}
