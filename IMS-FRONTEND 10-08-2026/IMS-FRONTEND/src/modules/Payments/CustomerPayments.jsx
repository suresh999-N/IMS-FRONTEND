import {
  createCustomerPayment,
  deleteCustomerPayment,
  getCustomerPayments,
  updateCustomerPayment,
} from '../../api/businessApi'
import CustomerPaymentModule from './PaymentModule'

export default function CustomerPayments({ customers }) {
  return (
    <CustomerPaymentModule
      customers={customers}
      fetchPayments={getCustomerPayments}
      createPayment={createCustomerPayment}
      updatePayment={updateCustomerPayment}
      deletePayment={deleteCustomerPayment}
    />
  )
}
