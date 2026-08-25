// Neon Courier Transit reminder: the route contains only the full-screen game frame; no generic page chrome competes with play.

import ErrorBoundary from "./components/ErrorBoundary";
import GameCanvas from "./components/GameCanvas";

function App() {
  return <ErrorBoundary><GameCanvas /></ErrorBoundary>;
}

export default App;
