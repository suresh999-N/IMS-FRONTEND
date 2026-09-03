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

export const STATE_PINCODE_PREFIXES = {
  'Andaman And Nicobar Islands': ['744'],
  'Andhra Pradesh': ['51', '52', '53'],
  'Arunachal Pradesh': ['790', '791', '792'],
  Assam: ['78'],
  Bihar: ['80', '81', '82', '83', '84', '85'],
  Chandigarh: ['160'],
  Chhattisgarh: ['49'],
  'Dadra And Nagar Haveli And Daman And Diu': ['396'],
  Delhi: ['11'],
  Goa: ['403'],
  Gujarat: ['36', '37', '38', '39'],
  Haryana: ['12', '13'],
  'Himachal Pradesh': ['17'],
  'Jammu And Kashmir': ['18', '19'],
  Jharkhand: ['81', '82', '83'],
  Karnataka: ['56', '57', '58', '59'],
  Kerala: ['67', '68', '69'],
  Ladakh: ['194'],
  Lakshadweep: ['682'],
  'Madhya Pradesh': ['45', '46', '47', '48'],
  Maharashtra: ['40', '41', '42', '43', '44'],
  Manipur: ['795'],
  Meghalaya: ['793', '794'],
  Mizoram: ['796'],
  Nagaland: ['797', '798'],
  Odisha: ['75', '76', '77'],
  Puducherry: ['605'],
  Punjab: ['14', '15', '16'],
  Rajasthan: ['30', '31', '32', '33', '34'],
  Sikkim: ['737'],
  'Tamil Nadu': ['60', '61', '62', '63', '64'],
  Telangana: ['50'],
  Tripura: ['799'],
  'Uttar Pradesh': ['20', '21', '22', '23', '24', '25', '26', '27', '28'],
  Uttarakhand: ['24', '25', '26'],
  'West Bengal': ['70', '71', '72', '73', '74'],
}

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
