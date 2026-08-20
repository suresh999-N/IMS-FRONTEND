import test from 'node:test'
import assert from 'node:assert/strict'
import { readFile } from 'node:fs/promises'

const registerSource = readFile(new URL('./Register.jsx', import.meta.url), 'utf8')
const authStyles = readFile(new URL('./Auth.css', import.meta.url), 'utf8')

test('all registration controls use the same normal-flow field group', async () => {
  const source = await registerSource

  assert.equal(source.match(/className="auth-register-field"/g)?.length, 5)
  for (const id of [
    'register-name',
    'register-email',
    'register-phone',
    'register-password',
    'register-confirm-password',
  ]) {
    assert.match(source, new RegExp(`className="auth-register-field"[\\s\\S]*?htmlFor="${id}"`))
  }
})

test('normal and error states keep compact, consistent spacing without clipping', async () => {
  const stylesheet = await authStyles

  assert.match(
    stylesheet,
    /\.auth-wrapper--register form\s*\{[^}]*display:\s*grid;[^}]*gap:\s*12px;/s,
  )
  assert.match(
    stylesheet,
    /\.auth-wrapper--register \.auth-register-field\s*\{[^}]*display:\s*grid;[^}]*gap:\s*6px;/s,
  )
  assert.doesNotMatch(stylesheet, /\.auth-wrapper--register \.auth-register-field[^}]*position:\s*absolute;/s)
})

test('all validation feedback uses one height-stable area above the form', async () => {
  const source = await registerSource
  const stylesheet = await authStyles

  assert.match(
    source,
    /isFormCompletelyEmpty[\s\S]*?"Please fill in all required fields\."[\s\S]*?validationErrors\.length === 1[\s\S]*?"Please correct the highlighted fields\."/,
  )
  assert.doesNotMatch(source, /field-error-text/)
  assert.doesNotMatch(source, /error-box auth-register-error/)
  assert.match(
    stylesheet,
    /\.auth-wrapper--register \.auth-register-error\s*\{[^}]*height:\s*24px;[^}]*padding:\s*0;[^}]*background:\s*transparent;[^}]*overflow:\s*hidden;/s,
  )
  assert.match(
    stylesheet,
    /\.auth-wrapper--register \.auth-register-error:not\(\.is-visible\)\s*\{[^}]*visibility:\s*hidden;/s,
  )
})

test('the registration subtitle is removed', async () => {
  const source = await registerSource

  assert.doesNotMatch(source, /Create your IMS workspace account/)
})

test('the decorative icon above the registration heading is removed', async () => {
  const source = await registerSource

  assert.doesNotMatch(source, /<div className="auth-login-lock"/)
  assert.match(source, /<h2>Create Account<\/h2>/)
})

test('the desktop form pane fits its compact content without scrolling', async () => {
  const stylesheet = await authStyles

  assert.match(
    stylesheet,
    /@media \(min-width:\s*1025px\)[\s\S]*?\.auth-wrapper--register \.auth-right-panel\s*\{[^}]*align-items:\s*center\s*!important;[^}]*overflow:\s*hidden\s*!important;/,
  )
  assert.doesNotMatch(stylesheet, /\.auth-wrapper--register \.auth-right-panel\s*\{[^}]*overflow-y:\s*auto/s)
})

test('password visibility controls remain wired to their existing handlers', async () => {
  const source = await registerSource

  assert.equal(source.match(/className="auth-login-password-toggle"/g)?.length, 2)
  assert.match(source, /setShowPassword\(\(current\) => !current\)/)
  assert.match(source, /setShowConfirmPassword\(\(current\) => !current\)/)
})
