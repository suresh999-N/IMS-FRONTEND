import { Plus, ScanLine } from 'lucide-react'
import { useState, useEffect, useMemo } from 'react'
import { useAuth } from '../../../hooks/useAuth'
import PageHeader from '../../../components/common/PageHeader'
import StateBlock from '../../../components/common/StateBlock'
import { ConfirmationDialog } from '../../../components/erp'
import { getRequiredError, getToday } from '../../../utils/helpers'
import BarcodeForm from './components/BarcodeForm'
import BarcodeTable from './components/BarcodeTable'
import { getPreviewValue } from './utils/preview'
import { getBarcodes, generateBarcode, findExistingBarcode } from '../../../api/barcodeApi'
import './Barcode.css'

const initialForm = {
  productId: '',
  codeType: 'Barcode',
  date: getToday(),
}

export default function Barcode({
  products = [],
  onQuickAddProduct,
}) {
  const { hasPermission } = useAuth()
  const [barcodes, setBarcodes] = useState([])
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [formData, setFormData] = useState(initialForm)
  const [touched, setTouched] = useState({})
  const [message, setMessage] = useState(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [errorMessage, setErrorMessage] = useState('')
  const [showOverwriteConfirm, setShowOverwriteConfirm] = useState(false)
  const canCreate = hasPermission('barcode', 'create')

  const errors = {
    productId: getRequiredError(formData.productId, 'Product'),
    date: getRequiredError(formData.date, 'Date'),
  }
  const selectedProduct = products.find((item) => String(item.id) === String(formData.productId)) ?? null
  const livePreviewValue = getPreviewValue(selectedProduct, formData.codeType)

  const existingBarcode = useMemo(() => {
    if (!formData.productId && !selectedProduct?.name) return null
    return findExistingBarcode(formData.productId, selectedProduct?.name, barcodes)
  }, [barcodes, formData.productId, selectedProduct])

  async function loadBarcodes() {
    setIsLoading(true)
    setErrorMessage('')
    try {
      const response = await getBarcodes(products)
      if (response.success) {
        setBarcodes(response.data)
      } else {
        setErrorMessage(response.error || 'Failed to load barcodes.')
      }
    } catch (err) {
      setErrorMessage('An unexpected error occurred while loading barcodes.')
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    loadBarcodes()
  }, [products])

  useEffect(() => {
    if (message) {
      const timer = setTimeout(() => {
        setMessage(null)
      }, 4000)
      return () => clearTimeout(timer)
    }
  }, [message])

  function handleChange(event) {
    const { name, value } = event.target
    setFormData((currentValue) => ({ ...currentValue, [name]: value }))
  }

  function handleBlur(event) {
    const { name } = event.target
    setTouched((currentValue) => ({ ...currentValue, [name]: true }))
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setTouched({ productId: true, date: true })

    if (Object.values(errors).some(Boolean)) {
      return
    }

    if (existingBarcode) {
      setShowOverwriteConfirm(true)
      return
    }

    await executeGenerate(false)
  }

  async function executeGenerate(isOverwrite = false) {
    setIsSaving(true)
    setMessage(null)

    try {
      const response = await generateBarcode(formData.productId, products)

      if (response.success) {
        setBarcodes((current) => {
          if (isOverwrite) {
            const filtered = current.filter((b) => {
              const isMatch =
                (formData.productId && String(b.productId) === String(formData.productId)) ||
                (selectedProduct?.name && b.productName && b.productName.trim().toLowerCase() === selectedProduct.name.trim().toLowerCase())
              return !isMatch
            })
            return [response.data, ...filtered]
          }
          return [response.data, ...current]
        })
        setFormData(initialForm)
        setTouched({})
        setIsFormOpen(false)
        setShowOverwriteConfirm(false)
        setMessage({
          success: true,
          message: `${formData.codeType} ${isOverwrite ? 'regenerated and overwritten' : 'generated'} successfully.`,
        })
      } else {
        setMessage({
          success: false,
          message: response.error || 'Failed to generate code.',
        })
      }
    } catch (err) {
      setMessage({
        success: false,
        message: 'An unexpected error occurred while generating code.',
      })
    } finally {
      setIsSaving(false)
    }
  }

  function handleCancelOverwrite() {
    setShowOverwriteConfirm(false)
    setMessage({
      success: false,
      message: 'Barcode already exists for this product.',
    })
  }

  function handleQuickAddProduct(values) {
    const result = onQuickAddProduct(values)
    setMessage(result)
    return result.success ? result.item : null
  }

  function handleCancel() {
    setFormData(initialForm)
    setTouched({})
    setIsFormOpen(false)
    setShowOverwriteConfirm(false)
  }

  return (
    <div className="page barcode-page">
      <PageHeader
        icon={ScanLine}
        title="Barcode / QR"
        description=""
        actions={
          canCreate ? (
            <button
              type="button"
              className="button button-primary"
              onClick={() => setIsFormOpen((currentValue) => !currentValue)}
            >
              <Plus size={16} />
              Add Code
            </button>
          ) : null
        }
      />

      {message ? (
        <div
          className={`message-box ${
            message.success ? 'message-box--success' : 'message-box--error'
          }`}
        >
          {message.message}
        </div>
      ) : null}

      {showOverwriteConfirm ? (
        <ConfirmationDialog
          title="Barcode Already Exists"
          message={`A barcode already exists for "${selectedProduct?.name || 'this product'}". Do you want to regenerate and overwrite the existing barcode?`}
          confirmLabel="Overwrite & Regenerate"
          cancelLabel="Cancel"
          tone="warning"
          isSubmitting={isSaving}
          onConfirm={() => executeGenerate(true)}
          onCancel={handleCancelOverwrite}
        />
      ) : null}

      {isFormOpen ? (
        <BarcodeForm
          formData={formData}
          touched={touched}
          errors={errors}
          products={products}
          livePreviewValue={livePreviewValue}
          existingBarcode={existingBarcode}
          onChange={handleChange}
          onBlur={handleBlur}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          onQuickAddProduct={handleQuickAddProduct}
          isSaving={isSaving}
        />
      ) : null}

      {isLoading ? (
        <StateBlock
          type="loading"
          title="Loading Barcodes"
          message="Fetching dynamic barcodes from the backend..."
        />
      ) : errorMessage ? (
        <StateBlock
          type="error"
          title="Failed to Load Barcodes"
          message={errorMessage}
          actionLabel="Retry"
          onAction={loadBarcodes}
        />
      ) : (
        <BarcodeTable barcodes={barcodes} onRefresh={loadBarcodes} isLoading={isLoading} />
      )}
    </div>
  )
}
