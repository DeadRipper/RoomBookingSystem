import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import "./index.css";
import App from "./App.tsx";
import { BookingsProvider } from "./store/BookingsContext";
import { RoomsProvider } from "./store/RoomsContext";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <RoomsProvider>
        <BookingsProvider>
          <App />
        </BookingsProvider>
      </RoomsProvider>
    </BrowserRouter>
  </StrictMode>,
);
