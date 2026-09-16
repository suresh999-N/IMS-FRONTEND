import {
  FOREIGN_STATES_AND_COUNTRIES,
  INDIA_STATES,
  MAJOR_CITIES_BY_STATE,
  CITY_TO_STATE_MAP,
} from '../modules/Suppliers/supplierMasterData'

export { FOREIGN_STATES_AND_COUNTRIES, INDIA_STATES, MAJOR_CITIES_BY_STATE, CITY_TO_STATE_MAP }

export function getCityStateError(city, state, country = 'India') {
  const cleanCity = String(city ?? '').trim()
  const cleanState = String(state ?? '').trim()

  if (!cleanCity || !cleanState) return ''

  const normalizeKey = (str) => String(str ?? '').replace(/[\s\-_]/g, '').toLowerCase()
  const keyCity = normalizeKey(cleanCity)
  const keyState = normalizeKey(cleanState)

  // 1. Check if the entered city is actually an Indian State or UT name
  const matchedIndianState = INDIA_STATES.find((s) => normalizeKey(s) === keyCity)
  if (matchedIndianState) {
    if (normalizeKey(matchedIndianState) !== keyState) {
      return `'${cleanCity}' is a state name, not a city in ${cleanState}.`
    }
  }

  // 2. Check if the entered city is a foreign state/province or country name
  const matchedForeignState = FOREIGN_STATES_AND_COUNTRIES.find((s) => normalizeKey(s) === keyCity)
  if (matchedForeignState) {
    return `'${cleanCity}' is not a valid city in ${cleanState}.`
  }

  // 3. Check registered city-to-state mapping
  const registeredState = CITY_TO_STATE_MAP[keyCity]
  if (registeredState && normalizeKey(registeredState) !== keyState) {
    return `${cleanCity} belongs to ${registeredState}, not ${cleanState}.`
  }

  return ''
}
