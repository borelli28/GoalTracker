import './global.css'

export const metadata = {
  title: 'Daily Trackerr',
  description: 'Track your productivity over time',
}

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  )
}
