import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import { statusDisplayer } from "../helpers/statusDisplayer";
export const PackagePage = () => {
    const [item, setItem] = useState({
        id: "",
        trackingNumber: "",
        sender: {
            name: "",
            phone: "",
            address: ""
        },
        recipient: {
            name: "",
            phone: "",
            address: ""
        },
        currentStatus: 0,
        currentStatusTimestamp: "",
        history: [
            {
            status: 0,
            date: ""
            }
        ],
        allowedTransitions: []
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchPackages = async () => {
            try {
                setError("");
                const id = window.location.pathname.split("/").pop();
                const res = await axios.get(`http://localhost:5196/packages/${id}`);
                setItem(res.data);
            } catch (err) {
                setError("Failed to fetch packages");
            } finally {
                setLoading(false);
            }
        }
        fetchPackages();
    }, [loading]);

    const handleChangeStatus = async (newStatus) => {
        try {
            setError("");
            setLoading(true);
            await axios.post(`http://localhost:5196/packages/${item.id}/status`, { nextStatus: newStatus });
        } catch (err) {
            setError("Failed to change status");
            console.error(err);
        } finally {
            setLoading(true);
        }
    }


    return (
        <main className="package-page">
            <Link to={"/"}>Back</Link>
            <h1>Package Info</h1>
            {loading && <p>Loading...</p>}
            {error && <p>{error}</p>}
            <div>
                <p>Tracking Number: {item.trackingNumber}</p>
                <p>Sender: {item.sender.name} ({item.sender.phone})</p>
                <p>Address: {item.sender.address}</p>
                <p>Recipient: {item.recipient.name} ({item.recipient.phone})</p>
                <p>Address: {item.recipient.address}</p>
                <p>Status: {statusDisplayer(item.currentStatus)}</p>
                <p>Date Created: {new Date(item.history[item.history.length - 1].date).toLocaleDateString()}</p>
                <div>
                    <h3>Change Status</h3>
                    {item.allowedTransitions.map((status) => (
                        <button key={status} onClick={() => handleChangeStatus(status)}>
                            Change to {statusDisplayer(status)}
                        </button>
                    ))}
                </div>
                
                <h2>History</h2>
                <ul>
                    {item.history.map((event, index) => (
                        <li key={index}>
                            {event.status} on {new Date(event.date).toLocaleDateString()}
                        </li>
                    ))}
                </ul>
            </div>
        </main>
    );
}