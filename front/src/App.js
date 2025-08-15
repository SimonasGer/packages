import { BrowserRouter as Router, Routes, Route} from "react-router-dom";
import { Main } from "./pages/Main";
import { PackagePage } from "./pages/PackagePage";
import "./style.scss";
function App() {
  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path="/" element={<Main/>}/>
          <Route path="/package/:id" element={<PackagePage/>}/>
        </Routes>
      </Router>
    </div>
  );
}

export default App;
