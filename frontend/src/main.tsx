import React from "react";
// import Playground from "./Playground";
import ReactDOM from "react-dom/client";
import AppRoutes from "./routes/AppRoutes";
import { ThemeModeProvider } from "./theme/ThemeContext";


// import './index.css'
// import App from './App.tsx'

// const theme = createTheme({
//   palette: {
//     mode: "light", // o "dark"
//     background: {
//       default: "#f5f5f5",
//     },
//   },
// });

// createRoot(document.getElementById('root')!).render(
//   <ThemeProvider theme={theme}>
//     <StrictMode>
//       <CssBaseline />
//       {/* <App /> */}
//       {/* <Playground /> */}
//       <AppRoutes />
//     </StrictMode>
//   </ThemeProvider>,
// )
ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ThemeModeProvider>
      <AppRoutes />
       {/* <App /> */}
       {/* <Playground /> */}
    </ThemeModeProvider>
  </React.StrictMode>
);