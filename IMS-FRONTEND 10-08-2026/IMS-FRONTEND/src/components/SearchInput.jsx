import { useEffect, useState } from 'react'
import { Search } from 'lucide-react'
import './SearchInput.css'

export default function SearchInput({
  value,
  onChange,
  placeholder = 'Search...',
  debounce = 200,
}) {
  const [internalValue, setInternalValue] = useState(value ?? '')

  useEffect(() => {
    setInternalValue(value ?? '')
  }, [value])

  useEffect(() => {
    const timer = setTimeout(() => {
      onChange(internalValue)
    }, debounce)

    return () => clearTimeout(timer)
  }, [internalValue, onChange, debounce])

  return (
    <div className="search-input-container">
      <div className="search-wrapper">
        <Search className="search-icon" size={16} />
        <input
          type="search"
          value={internalValue}
          onChange={(event) => setInternalValue(event.target.value)}
          placeholder={placeholder}
          aria-label={placeholder}
          autoComplete="off"
        />
      </div>
    </div>
  )
}
