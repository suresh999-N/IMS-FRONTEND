import {
  FOREIGN_STATES_AND_COUNTRIES,
  INDIA_STATES,
  MAJOR_CITIES_BY_STATE,
  CITY_TO_STATE_MAP,
  getCityStateError as masterGetCityStateError,
} from '../modules/Suppliers/supplierMasterData'

export { FOREIGN_STATES_AND_COUNTRIES, INDIA_STATES, MAJOR_CITIES_BY_STATE, CITY_TO_STATE_MAP }

export function getCityStateError(city, state, country = 'India') {
  return masterGetCityStateError(city, state, country)
}
