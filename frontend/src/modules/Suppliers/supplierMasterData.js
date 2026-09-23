export const IFSC_PATTERN = /^[A-Z]{4}0[A-Z0-9]{6}$/

export const INDIA_STATES = [
  'Andaman And Nicobar Islands', 'Andhra Pradesh', 'Arunachal Pradesh', 'Assam', 'Bihar', 'Chandigarh',
  'Chhattisgarh', 'Dadra And Nagar Haveli And Daman And Diu', 'Delhi', 'Goa', 'Gujarat', 'Haryana',
  'Himachal Pradesh', 'Jammu And Kashmir', 'Jharkhand', 'Karnataka', 'Kerala', 'Ladakh', 'Lakshadweep',
  'Madhya Pradesh', 'Maharashtra', 'Manipur', 'Meghalaya', 'Mizoram', 'Nagaland', 'Odisha',
  'Puducherry', 'Punjab', 'Rajasthan', 'Sikkim', 'Tamil Nadu', 'Telangana', 'Tripura',
  'Uttar Pradesh', 'Uttarakhand', 'West Bengal',
]

export const INDIAN_STATES = INDIA_STATES

export const COUNTRY_OPTIONS = [
  'India',
  'United States',
  'United Kingdom',
  'United Arab Emirates',
  'Australia',
  'Canada',
  'Singapore',
  'Germany',
  'France',
  'Japan',
  'China',
  'South Korea',
  'Malaysia',
  'Thailand',
  'Indonesia',
  'Vietnam',
  'Bangladesh',
  'Sri Lanka',
  'Nepal',
  'Bhutan',
  'Saudi Arabia',
  'Qatar',
  'Oman',
  'Kuwait',
  'South Africa',
  'Brazil',
  'Mexico',
  'Netherlands',
  'Italy',
  'Spain',
]

import { STATE_PINCODE_PREFIXES } from '../../validators/pincodeValidator.js'
export { STATE_PINCODE_PREFIXES }

export const DEPARTMENT_OPTIONS = [
  'Procurement',
  'Finance',
  'Logistics',
  'Operations',
  'Sales',
  'HR',
  'Compliance',
  'Legal',
  'IT',
  'Warehouse',
  'Accounts',
  'Vendor Management',
  'Administration',
]

export const DESIGNATION_OPTIONS = [
  'Procurement Manager',
  'Senior Procurement Manager',
  'Purchase Executive',
  'Finance Manager',
  'Accounts Executive',
  'Logistics Coordinator',
  'Supply Chain Manager',
  'Operations Lead',
  'Vendor Relationship Manager',
  'Compliance Officer',
  'Warehouse Supervisor',
  'Managing Director',
  'CEO',
  'CFO',
  'Branch Manager',
]

export const DESIGNATIONS_BY_DEPARTMENT = {
  Procurement: ['Procurement Manager', 'Senior Procurement Manager', 'Purchase Executive', 'Vendor Relationship Manager'],
  Finance: ['Finance Manager', 'Accounts Executive', 'CFO'],
  Accounts: ['Accounts Executive', 'Finance Manager'],
  Logistics: ['Logistics Coordinator', 'Supply Chain Manager'],
  Operations: ['Operations Lead', 'Branch Manager'],
  Warehouse: ['Warehouse Supervisor', 'Logistics Coordinator'],
  Compliance: ['Compliance Officer'],
  Legal: ['Compliance Officer'],
  Administration: ['Branch Manager', 'Managing Director'],
}

export const IFSC_REGISTRY = [
  ['SBIN0004786', 'State Bank Of India', 'Devanakonda', 'Andhra Pradesh', 'Kurnool'],
  ['SBIN0008754', 'State Bank Of India', 'Gachibowli', 'Telangana', 'Hyderabad'],
  ['SBIN0001234', 'State Bank Of India', 'Mumbai Main Branch', 'Maharashtra', 'Mumbai'],
  ['SBIN0011122', 'State Bank Of India', 'Koramangala', 'Karnataka', 'Bengaluru'],
  ['SBIN0022345', 'State Bank Of India', 'T Nagar', 'Tamil Nadu', 'Chennai'],
  ['SBIN0033456', 'State Bank Of India', 'Connaught Place', 'Delhi', 'New Delhi'],
  ['SBIN0044567', 'State Bank Of India', 'Salt Lake Sector V', 'West Bengal', 'Kolkata'],
  ['SBIN0055678', 'State Bank Of India', 'Vastrapur', 'Gujarat', 'Ahmedabad'],
  ['HDFC0001234', 'HDFC Bank', 'Bengaluru Main Branch', 'Karnataka', 'Bengaluru'],
  ['HDFC0004567', 'HDFC Bank', 'Madhapur Main Branch', 'Telangana', 'Hyderabad'],
  ['HDFC0008754', 'HDFC Bank', 'Andheri East', 'Maharashtra', 'Mumbai'],
  ['HDFC0011023', 'HDFC Bank', 'Anna Nagar', 'Tamil Nadu', 'Chennai'],
  ['HDFC0022045', 'HDFC Bank', 'Cyber City', 'Haryana', 'Gurugram'],
  ['HDFC0033067', 'HDFC Bank', 'Park Street', 'West Bengal', 'Kolkata'],
  ['HDFC0044089', 'HDFC Bank', 'C G Road', 'Gujarat', 'Ahmedabad'],
  ['HDFC0055012', 'HDFC Bank', 'Civil Lines', 'Rajasthan', 'Jaipur'],
  ['ICIC0005678', 'ICICI Bank', 'Begumpet', 'Telangana', 'Hyderabad'],
  ['ICIC0009988', 'ICICI Bank', 'M.G. Road', 'Karnataka', 'Bengaluru'],
  ['ICIC0012345', 'ICICI Bank', 'Bandra Kurla Complex', 'Maharashtra', 'Mumbai'],
  ['ICIC0023456', 'ICICI Bank', 'Adyar', 'Tamil Nadu', 'Chennai'],
  ['ICIC0034567', 'ICICI Bank', 'Nehru Place', 'Delhi', 'New Delhi'],
  ['ICIC0045678', 'ICICI Bank', 'Koregaon Park', 'Maharashtra', 'Pune'],
  ['ICIC0056789', 'ICICI Bank', 'Alkapuri', 'Gujarat', 'Vadodara'],
  ['ICIC0067890', 'ICICI Bank', 'Sector 17', 'Chandigarh', 'Chandigarh'],
  ['UTIB0001023', 'Axis Bank', 'Madhapur', 'Telangana', 'Hyderabad'],
  ['UTIB0002045', 'Axis Bank', 'Whitefield', 'Karnataka', 'Bengaluru'],
  ['UTIB0003067', 'Axis Bank', 'Powai', 'Maharashtra', 'Mumbai'],
  ['UTIB0004089', 'Axis Bank', 'Velachery', 'Tamil Nadu', 'Chennai'],
  ['UTIB0005012', 'Axis Bank', 'Janakpuri', 'Delhi', 'New Delhi'],
  ['UTIB0006034', 'Axis Bank', 'Marine Lines', 'Maharashtra', 'Mumbai'],
  ['PUNB0001234', 'Punjab National Bank', 'Karol Bagh', 'Delhi', 'New Delhi'],
  ['PUNB0012345', 'Punjab National Bank', 'Ameerpet', 'Telangana', 'Hyderabad'],
  ['PUNB0023456', 'Punjab National Bank', 'Rajajinagar', 'Karnataka', 'Bengaluru'],
  ['PUNB0034567', 'Punjab National Bank', 'Ludhiana Main', 'Punjab', 'Ludhiana'],
  ['PUNB0045678', 'Punjab National Bank', 'MI Road', 'Rajasthan', 'Jaipur'],
  ['PUNB0056789', 'Punjab National Bank', 'Hazratganj', 'Uttar Pradesh', 'Lucknow'],
  ['CNRB0001111', 'Canara Bank', 'Jayanagar', 'Karnataka', 'Bengaluru'],
  ['CNRB0002222', 'Canara Bank', 'Kukatpally', 'Telangana', 'Hyderabad'],
  ['CNRB0003333', 'Canara Bank', 'Dadar West', 'Maharashtra', 'Mumbai'],
  ['CNRB0004444', 'Canara Bank', 'Coimbatore Main', 'Tamil Nadu', 'Coimbatore'],
  ['CNRB0005555', 'Canara Bank', 'Panaji', 'Goa', 'Panaji'],
  ['CNRB0006666', 'Canara Bank', 'Kochi Main', 'Kerala', 'Kochi'],
  ['UBIN0001001', 'Union Bank Of India', 'Secunderabad', 'Telangana', 'Hyderabad'],
  ['UBIN0002002', 'Union Bank Of India', 'Indiranagar', 'Karnataka', 'Bengaluru'],
  ['UBIN0003003', 'Union Bank Of India', 'Fort Mumbai', 'Maharashtra', 'Mumbai'],
  ['UBIN0004004', 'Union Bank Of India', 'Mount Road', 'Tamil Nadu', 'Chennai'],
  ['UBIN0005005', 'Union Bank Of India', 'Bhubaneswar Main', 'Odisha', 'Bhubaneswar'],
  ['UBIN0006006', 'Union Bank Of India', 'Patna Main', 'Bihar', 'Patna'],
  ['KKBK0001111', 'Kotak Mahindra Bank', 'Hi Tech City', 'Telangana', 'Hyderabad'],
  ['KKBK0002222', 'Kotak Mahindra Bank', 'Malleshwaram', 'Karnataka', 'Bengaluru'],
  ['KKBK0003333', 'Kotak Mahindra Bank', 'Nariman Point', 'Maharashtra', 'Mumbai'],
  ['KKBK0004444', 'Kotak Mahindra Bank', 'Nungambakkam', 'Tamil Nadu', 'Chennai'],
  ['KKBK0005555', 'Kotak Mahindra Bank', 'Sector 18', 'Uttar Pradesh', 'Noida'],
  ['INDB0001234', 'IndusInd Bank', 'Jubilee Hills', 'Telangana', 'Hyderabad'],
  ['INDB0002345', 'IndusInd Bank', 'Electronic City', 'Karnataka', 'Bengaluru'],
  ['INDB0003456', 'IndusInd Bank', 'Lower Parel', 'Maharashtra', 'Mumbai'],
  ['INDB0004567', 'IndusInd Bank', 'Guindy', 'Tamil Nadu', 'Chennai'],
  ['ANDB0001111', 'Andhra Bank', 'Vijayawada Main', 'Andhra Pradesh', 'Vijayawada'],
  ['ANDB0002222', 'Andhra Bank', 'Guntur Main', 'Andhra Pradesh', 'Guntur'],
  ['ANDB0003333', 'Andhra Bank', 'Tirupati', 'Andhra Pradesh', 'Tirupati'],
  ['ANDB0004444', 'Andhra Bank', 'Warangal', 'Telangana', 'Warangal'],
  ['ANDB0005555', 'Andhra Bank', 'Rajahmundry', 'Andhra Pradesh', 'Rajahmundry'],
  ['BARB0001111', 'Bank Of Baroda', 'Banjara Hills', 'Telangana', 'Hyderabad'],
  ['BARB0002222', 'Bank Of Baroda', 'Peenya', 'Karnataka', 'Bengaluru'],
  ['BARB0003333', 'Bank Of Baroda', 'Borivali West', 'Maharashtra', 'Mumbai'],
  ['BARB0004444', 'Bank Of Baroda', 'Ashram Road', 'Gujarat', 'Ahmedabad'],
  ['IDIB0001111', 'Indian Bank', 'Mylapore', 'Tamil Nadu', 'Chennai'],
  ['IDIB0002222', 'Indian Bank', 'Dilsukhnagar', 'Telangana', 'Hyderabad'],
  ['IDIB0003333', 'Indian Bank', 'Mysuru Main', 'Karnataka', 'Mysuru'],
  ['IDIB0004444', 'Indian Bank', 'Thiruvananthapuram', 'Kerala', 'Thiruvananthapuram'],
].map(([ifscCode, bankName, branch, state, city]) => ({
  ifscCode,
  bankName,
  branch,
  state,
  city,
}))

export const IFSC_DIRECTORY = Object.fromEntries(IFSC_REGISTRY.map((record) => [record.ifscCode, record]))

const BANK_PREFIXES = {
  SBIN: 'State Bank Of India',
  HDFC: 'HDFC Bank',
  ICIC: 'ICICI Bank',
  UTIB: 'Axis Bank',
  PUNB: 'Punjab National Bank',
  BARB: 'Bank Of Baroda',
  CNRB: 'Canara Bank',
  UBIN: 'Union Bank Of India',
  IDIB: 'Indian Bank',
  KKBK: 'Kotak Mahindra Bank',
  INDB: 'IndusInd Bank',
  ANDB: 'Andhra Bank',
  YESB: 'YES Bank',
}

const ifscLookupCache = new Map()

export function toOptions(values) {
  return values.map((value) => ({ value, label: value }))
}

export function mergeMasterOptions(...groups) {
  const byValue = new Map()

  groups.flat().filter(Boolean).forEach((item) => {
    const value = typeof item === 'string' ? item : item.value
    const label = typeof item === 'string' ? item : item.label || item.value
    const key = String(value || '').trim().toLowerCase()

    if (key && !byValue.has(key)) {
      byValue.set(key, { value, label })
    }
  })

  return [...byValue.values()]
}

export function getDesignationOptionsForDepartment(department) {
  const prioritized = DESIGNATIONS_BY_DEPARTMENT[department] || []
  return mergeMasterOptions(prioritized, DESIGNATION_OPTIONS)
}

export function getIfscDetails(ifscCode) {
  const normalizedIfsc = String(ifscCode ?? '').trim().toUpperCase()
  if (!IFSC_PATTERN.test(normalizedIfsc)) return null
  if (ifscLookupCache.has(normalizedIfsc)) return ifscLookupCache.get(normalizedIfsc)

  const exactDetails = IFSC_DIRECTORY[normalizedIfsc] || null
  const result = exactDetails
    ? { status: 'recognized', ...exactDetails }
    : {
        status: 'unrecognized',
        ifscCode: normalizedIfsc,
        bankName: getBankNameForIfscPrefix(normalizedIfsc),
        branch: '',
        state: '',
        city: '',
      }

  ifscLookupCache.set(normalizedIfsc, result)
  return result
}

export function getBankNameForIfscPrefix(ifscCode) {
  const prefix = String(ifscCode ?? '').trim().toUpperCase().slice(0, 4)
  return BANK_PREFIXES[prefix] || ''
}

export const FOREIGN_STATES_AND_COUNTRIES = [
  // USA — states
  'Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California', 'Colorado', 'Connecticut',
  'Delaware', 'Florida', 'Georgia', 'Hawaii', 'Idaho', 'Illinois', 'Indiana', 'Iowa',
  'Kansas', 'Kentucky', 'Louisiana', 'Maine', 'Maryland', 'Massachusetts', 'Michigan',
  'Minnesota', 'Mississippi', 'Missouri', 'Montana', 'Nebraska', 'Nevada', 'New Hampshire',
  'New Jersey', 'New Mexico', 'New York', 'Newyork', 'North Carolina', 'North Dakota',
  'Ohio', 'Oklahoma', 'Oregon', 'Pennsylvania', 'Rhode Island', 'South Carolina',
  'South Dakota', 'Tennessee', 'Texas', 'Utah', 'Vermont', 'Virginia', 'Washington',
  'West Virginia', 'Wisconsin', 'Wyoming',
  // USA — major cities
  'Dallas', 'Houston', 'Chicago', 'Los Angeles', 'San Francisco', 'San Jose', 'San Diego',
  'San Antonio', 'Miami', 'Seattle', 'Boston', 'Austin', 'Denver', 'Phoenix', 'Philadelphia',
  'Atlanta', 'Nashville', 'Portland', 'Las Vegas', 'Minneapolis', 'New Orleans', 'Detroit',
  'Baltimore', 'Memphis', 'Louisville', 'Milwaukee', 'Albuquerque', 'Tucson', 'Fresno',
  'Sacramento', 'Kansas City', 'Mesa', 'Omaha', 'Colorado Springs', 'Raleigh', 'Arlington',
  'Long Beach', 'Tampa', 'Honolulu', 'Anaheim',
  // UK
  'London', 'Manchester', 'Birmingham', 'Leeds', 'Glasgow', 'Sheffield', 'Bradford',
  'Edinburgh', 'Liverpool', 'Bristol', 'Cardiff', 'Belfast', 'Leicester', 'Coventry',
  'Nottingham', 'Newcastle', 'Southampton', 'Brighton', 'Plymouth', 'Oxford', 'Cambridge',
  // Europe
  'Paris', 'Berlin', 'Madrid', 'Rome', 'Barcelona', 'Amsterdam', 'Brussels', 'Vienna',
  'Warsaw', 'Budapest', 'Prague', 'Lisbon', 'Athens', 'Stockholm', 'Copenhagen', 'Helsinki',
  'Oslo', 'Zurich', 'Geneva', 'Milan', 'Naples', 'Munich', 'Hamburg', 'Frankfurt',
  'Lyon', 'Marseille', 'Bordeaux', 'Rotterdam', 'Antwerp', 'Cologne', 'Stuttgart', 'Krakow',
  // Middle East & Africa
  'Dubai', 'Abu Dhabi', 'Riyadh', 'Jeddah', 'Muscat', 'Doha', 'Manama',
  'Cairo', 'Lagos', 'Nairobi', 'Casablanca', 'Accra', 'Johannesburg', 'Cape Town',
  'Addis Ababa', 'Beirut', 'Amman', 'Baghdad', 'Tehran', 'Tel Aviv', 'Jerusalem',
  // Asia Pacific
  'Tokyo', 'Osaka', 'Kyoto', 'Beijing', 'Shanghai', 'Guangzhou', 'Shenzhen', 'Chengdu',
  'Hong Kong', 'Singapore', 'Seoul', 'Busan', 'Taipei', 'Bangkok', 'Jakarta', 'Kuala Lumpur',
  'Manila', 'Ho Chi Minh City', 'Hanoi', 'Yangon', 'Colombo', 'Dhaka', 'Karachi',
  'Lahore', 'Islamabad', 'Kathmandu', 'Ulaanbaatar',
  // Canada & Australia
  'Toronto', 'Vancouver', 'Montreal', 'Calgary', 'Edmonton', 'Ottawa', 'Winnipeg', 'Quebec',
  'Ontario', 'Sydney', 'Melbourne', 'Brisbane', 'Perth', 'Adelaide', 'Auckland', 'Wellington',
  // Latin America
  'Mexico City', 'Sao Paulo', 'Rio de Janeiro', 'Buenos Aires', 'Lima', 'Bogota', 'Santiago',
  'Caracas', 'Quito', 'La Paz', 'Montevideo', 'Havana',
  // Russia & Central Asia
  'Moscow', 'Saint Petersburg', 'Novosibirsk', 'Yekaterinburg', 'Tashkent', 'Almaty', 'Baku',
]

// Pre-computed Set for O(1) lookup — normalised (no spaces/hyphens, lowercase)
const FOREIGN_CITY_KEYS = new Set(
  FOREIGN_STATES_AND_COUNTRIES.map((s) => s.replace(/[\s\-_]/g, '').toLowerCase())
)


export const MAJOR_CITIES_BY_STATE = {
  'Andhra Pradesh': ['Visakhapatnam', 'Vijayawada', 'Guntur', 'Nellore', 'Kurnool', 'Kakinada', 'Rajahmundry', 'Rajamahendravaram', 'Tirupati', 'Anantapur', 'Kadapa', 'Eluru', 'Vizianagaram', 'Machilipatnam', 'Tenali', 'Ongole', 'Nandyal', 'Chittoor', 'Devanakonda'],
  Telangana: ['Hyderabad', 'Warangal', 'Nizamabad', 'Khammam', 'Karimnagar', 'Ramagundam', 'Mahbubnagar', 'Nalgonda', 'Adilabad', 'Secunderabad', 'Gachibowli', 'Madhapur', 'Begumpet', 'Kukatpally'],
  Karnataka: ['Bengaluru', 'Bangalore', 'Mysore', 'Mysuru', 'Hubballi', 'Dharwad', 'Mangaluru', 'Mangalore', 'Belagavi', 'Belgaum', 'Davanagere', 'Ballari', 'Bellary', 'Tumakuru', 'Shivamogga', 'Koramangala', 'Whitefield', 'Indiranagar', 'Jayanagar', 'Rajajinagar'],
  Maharashtra: ['Mumbai', 'Pune', 'Nagpur', 'Thane', 'Pimpri Chinchwad', 'Nashik', 'Kalyan', 'Dombivli', 'Vasai', 'Virar', 'Aurangabad', 'Navi Mumbai', 'Solapur', 'Mira Bhayandar', 'Bhiwandi', 'Amravati', 'Nanded', 'Kolhapur', 'Sangli', 'Andheri', 'Bandra', 'Powai', 'Lower Parel'],
  'Tamil Nadu': ['Chennai', 'Coimbatore', 'Madurai', 'Tiruchirappalli', 'Trichy', 'Salem', 'Tiruppur', 'Erode', 'Vellore', 'Tirunelveli', 'Thoothukudi', 'Adyar', 'Mylapore', 'Nungambakkam', 'Velachery', 'Anna Nagar'],
  Delhi: ['New Delhi', 'Delhi', 'Central Delhi', 'East Delhi', 'North Delhi', 'South Delhi', 'West Delhi', 'Connaught Place', 'Karol Bagh', 'Janakpuri', 'Nehru Place'],
  Gujarat: ['Ahmedabad', 'Surat', 'Vadodara', 'Baroda', 'Rajkot', 'Bhavnagar', 'Jamnagar', 'Junagadh', 'Gandhinagar', 'Vastrapur', 'Alkapuri', 'Ashram Road'],
  'West Bengal': ['Kolkata', 'Calcutta', 'Asansol', 'Siliguri', 'Durgapur', 'Bardhaman', 'Malda', 'Baharampur', 'Kharagpur', 'Salt Lake'],
  'Uttar Pradesh': ['Lucknow', 'Kanpur', 'Ghaziabad', 'Agra', 'Meerut', 'Varanasi', 'Prayagraj', 'Allahabad', 'Bareilly', 'Aligarh', 'Moradabad', 'Saharanpur', 'Gorakhpur', 'Noida', 'Greater Noida', 'Hazratganj'],
  Rajasthan: ['Jaipur', 'Jodhpur', 'Kota', 'Bikaner', 'Ajmer', 'Udaipur', 'Bhilwara', 'Alwar'],
  Punjab: ['Ludhiana', 'Amritsar', 'Jalandhar', 'Patiala', 'Bathinda', 'Mohali'],
  Haryana: ['Gurugram', 'Gurgaon', 'Faridabad', 'Panipat', 'Ambala', 'Yamunanagar', 'Rohtak', 'Hisar', 'Karnal', 'Cyber City'],
  Kerala: ['Thiruvananthapuram', 'Kochi', 'Cochin', 'Kozhikode', 'Calicut', 'Kollam', 'Thrissur', 'Kannur', 'Alappuzha'],
  'Madhya Pradesh': ['Indore', 'Bhopal', 'Jabalpur', 'Gwalior', 'Ujjain', 'Sagar'],
  Bihar: ['Patna', 'Gaya', 'Bhagalpur', 'Muzaffarpur', 'Purnia', 'Darbhanga'],
  Assam: ['Guwahati', 'Silchar', 'Dibrugarh', 'Jorhat', 'Nagaon', 'Tinsukia'],
  Odisha: ['Bhubaneswar', 'Cuttack', 'Rourkela', 'Berhampur', 'Sambalpur'],
  Goa: ['Panaji', 'Margao', 'Vasco da Gama', 'Mapusa'],
  Puducherry: ['Puducherry', 'Pondicherry', 'Karaikal'],
  Chandigarh: ['Chandigarh'],
  'Himachal Pradesh': ['Shimla', 'Dharamshala', 'Mandi', 'Solan'],
  'Jammu And Kashmir': ['Srinagar', 'Jammu', 'Anantnag'],
  Jharkhand: ['Ranchi', 'Jamshedpur', 'Dhanbad', 'Bokaro'],
  Chhattisgarh: ['Raipur', 'Bhilai', 'Bilaspur', 'Korba'],
  Uttarakhand: ['Dehradun', 'Haridwar', 'Roorkee', 'Haldwani'],
}

export const CITY_TO_STATE_MAP = {}
Object.entries(MAJOR_CITIES_BY_STATE).forEach(([state, cities]) => {
  cities.forEach((city) => {
    const key = city.replace(/[\s\-_]/g, '').toLowerCase()
    CITY_TO_STATE_MAP[key] = state
  })
})

export function getCityStateError(city, state, country = 'India') {
  const cleanCity = String(city ?? '').trim()
  const cleanState = String(state ?? '').trim()
  const cleanCountry = String(country ?? 'India').trim()

  if (!cleanCity || !cleanState) return ''

  const normalizeKey = (str) => String(str ?? '').replace(/[\s\-_]/g, '').toLowerCase()
  const keyCity = normalizeKey(cleanCity)
  const keyState = normalizeKey(cleanState)
  const isIndia = cleanCountry.toLowerCase() === 'india'

  // Rule 1: City typed is actually an Indian state name
  const matchedIndianState = INDIA_STATES.find((s) => normalizeKey(s) === keyCity)
  if (matchedIndianState) {
    if (normalizeKey(matchedIndianState) !== keyState) {
      return `'${cleanCity}' is a state name, not a city in ${cleanState}.`
    }
  }

  // Rules 2 & 3 only apply when the country is India
  if (isIndia) {
    // Rule 2: City is a well-known foreign city/state/country
    if (FOREIGN_CITY_KEYS.has(keyCity)) {
      return `'${cleanCity}' is not a valid city in ${cleanState}, India. Please enter a valid Indian city.`
    }

    // Rule 3: City is registered in our India map but belongs to a different Indian state
    const registeredState = CITY_TO_STATE_MAP[keyCity]
    if (registeredState && normalizeKey(registeredState) !== keyState) {
      return `${cleanCity} belongs to ${registeredState}, not ${cleanState}.`
    }
  }

  return ''
}

