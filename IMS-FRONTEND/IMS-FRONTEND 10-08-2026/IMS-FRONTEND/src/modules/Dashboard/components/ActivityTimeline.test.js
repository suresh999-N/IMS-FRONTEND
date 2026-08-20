import test from 'node:test'
import assert from 'node:assert/strict'
import { readFile } from 'node:fs/promises'

test('every timeline entry is rendered with the shared activity timestamp formatter', async () => {
  const source = await readFile(new URL('./ActivityTimeline.jsx', import.meta.url), 'utf8')

  assert.match(source, /formatActivityTimestamp\(activity\.date\)/)
  assert.doesNotMatch(source, /formatRelativeTime\(activity\.date\)/)
})

test('activity timestamps stay together on desktop and wrap safely on small screens', async () => {
  const stylesheet = await readFile(new URL('../Dashboard.css', import.meta.url), 'utf8')

  assert.match(
    stylesheet,
    /\.activity-item__body time\s*\{[^}]*min-width:\s*0;[^}]*max-width:\s*100%;[^}]*white-space:\s*nowrap;/s,
  )
  assert.match(
    stylesheet,
    /@media \(max-width:\s*640px\)[\s\S]*?\.activity-item__body time\s*\{[^}]*white-space:\s*normal;[^}]*overflow-wrap:\s*anywhere;/,
  )
})
