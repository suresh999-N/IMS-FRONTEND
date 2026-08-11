import {
  createSupplierPayment,
  deleteSupplierPayment,
  getSupplierPayments,
} from '../../api/businessApi'
import SupplierPaymentModule from './SupplierPaymentModule'

export default function SupplierPayments({ suppliers }) {
  return (
    <SupplierPaymentModule
      suppliers={suppliers}
      fetchPayments={getSupplierPayments}
      createPayment={createSupplierPayment}
      deletePayment={deleteSupplierPayment}
    />
  )
}
