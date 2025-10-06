import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import Playground from "./Playground"
import './index.css'
// import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    {/* <App /> */}
    <Playground />
  </StrictMode>,
)
