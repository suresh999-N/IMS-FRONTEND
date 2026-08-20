import test from 'node:test'
import assert from 'node:assert/strict'
import {
  formatActivityDateTime,
  formatActivityTimestamp,
  formatRelativeTime,
} from './dateUtils.js'

const NOW = '2026-08-20T09:15:00.000Z'

test('formats an exact UTC timestamp in IST', () => {
  assert.equal(
    formatActivityDateTime('2026-08-17T09:15:00.000Z'),
    'Aug 17, 2026, 2:45 PM IST',
  )
})

test('converts across the IST calendar-day boundary', () => {
  assert.equal(
    formatActivityDateTime('2026-08-17T20:00:00.000Z'),
    'Aug 18, 2026, 1:30 AM IST',
  )
})

test('combines exact and relative values on one logical line', () => {
  assert.equal(
    formatActivityTimestamp('2026-08-17T09:15:00.000Z', NOW),
    'Aug 17, 2026, 2:45 PM IST · 3 days ago',
  )
})

test('formats past minutes, hours, days, and weeks naturally', () => {
  assert.equal(formatRelativeTime('2026-08-20T09:10:00.000Z', NOW), '5 minutes ago')
  assert.equal(formatRelativeTime('2026-08-20T07:15:00.000Z', NOW), '2 hours ago')
  assert.equal(formatRelativeTime('2026-08-19T08:15:00.000Z', NOW), 'yesterday')
  assert.equal(formatRelativeTime('2026-08-06T09:15:00.000Z', NOW), '2 weeks ago')
})

test('uses just now for recent and future timestamps without future-oriented wording', () => {
  assert.equal(formatRelativeTime('2026-08-20T09:14:30.000Z', NOW), 'just now')

  const futureResult = formatActivityTimestamp('2026-08-20T14:15:00.000Z', NOW)
  assert.match(futureResult, / · just now$/)
  assert.doesNotMatch(futureResult, /from now|\bin\s+\d+/i)
})

test('preserves timestamps containing UTC offsets before converting them to IST', () => {
  assert.equal(
    formatActivityTimestamp('2026-08-17T10:45:00.000+01:30', NOW),
    'Aug 17, 2026, 2:45 PM IST · 3 days ago',
  )
})

test('multiple activity entries use the same exact-plus-relative format', () => {
  const formattedEntries = [
    '2026-08-20T09:10:00.000Z',
    '2026-08-20T07:15:00.000Z',
    '2026-08-17T09:15:00.000Z',
  ].map((timestamp) => formatActivityTimestamp(timestamp, NOW))

  assert.deepEqual(formattedEntries, [
    'Aug 20, 2026, 2:40 PM IST · 5 minutes ago',
    'Aug 20, 2026, 12:45 PM IST · 2 hours ago',
    'Aug 17, 2026, 2:45 PM IST · 3 days ago',
  ])
})
