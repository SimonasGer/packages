import { useEffect, useState } from "react";
import axios from "axios";
import { List } from "../components/List";

export const Main = () => {
    const [packages, setPackages] = useState([]);
    const [newPackage, setNewPackage] = useState({
        senderName: "",
        senderPhone: "",
        senderAddress: "",
        recipientName: "",
        recipientPhone: "",
        recipientAddress: "",
    })
    const [status, setStatus] = useState("")
    const [dialogOpen, setDialogOpen] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchPackages = async () => {
            try {
                setError("");
                const res = await axios.get(`http://localhost:5196/packages?status=${status}`);
                setPackages(res.data);
            } catch (err) {
                setError("Failed to fetch packages");
            } finally {
                setLoading(false);
            }
        }
        fetchPackages();
    }, [loading, status]);

    const handleChange = (event) => {
        setNewPackage({
            ...newPackage,
            [event.target.name]: event.target.value
        });
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            setError("");
            await axios.post("http://localhost:5196/packages", newPackage);
            setDialogOpen(!dialogOpen)
        } catch (err) {
            setError("Failed to create package");
            console.error(err);
        } finally {
            setLoading(true);
        }
    }

    return (
        <main className="main">
            <dialog open={dialogOpen}>
                <h2>Dashboard</h2>
                <form onSubmit={handleSubmit}>
                    <fieldset>
                        <div>
                            <input type="text" name="senderName" placeholder="Sender Name" onChange={(event) => handleChange(event)}/>
                            <input type="text" name="senderPhone" placeholder="Sender Phone" onChange={(event) => handleChange(event)}/>
                            <input type="text" name="senderAddress" placeholder="Sender Address" onChange={(event) => handleChange(event)}/>
                        </div>
                        <div>
                            <input type="text" name="recipientName" placeholder="Recipient Name" onChange={(event) => handleChange(event)}/>
                            <input type="text" name="recipientPhone" placeholder="Recipient Phone" onChange={(event) => handleChange(event)}/>
                            <input type="text" name="recipientAddress" placeholder="Recipient Address" onChange={(event) => handleChange(event)}/>
                        </div>
                        <button type="submit">Create Package</button>
                    </fieldset>
                </form>
            </dialog>
            <h1>Packages</h1>
            <button onClick={() => setDialogOpen(!dialogOpen)}>Open/Close Dashboard</button>
            <select name="status" id="status" onChange={(e) => setStatus(e.target.value)} value={status}>
                <option value="">All</option>
                <option value="0">Created</option>
                <option value="1">Sent</option>
                <option value="2">Accepted</option>
                <option value="3">Returned</option>
                <option value="4">Canceled</option>
            </select>
            {loading && <p>Loading...</p>}
            {error && <p>{error}</p>}
            <List packages={packages} setLoading={setLoading}/>
        </main>
    );
}