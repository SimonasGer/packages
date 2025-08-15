import { useNavigate } from "react-router-dom";
import { useState } from "react";
import axios from "axios";
export const ListItem = (props) => {
    const navigate = useNavigate();
    const handleNav = () => {
        navigate(`/package/${props.id}`);
    }

    const handleChangeStatus = async (newStatus) => {
        try {
            await axios.post(`http://localhost:5196/packages/${props.id}/status`, { nextStatus: newStatus });
        } catch (err) {
            //props.setError("Failed to change status");
            console.error(err);
        } finally {
            //props.setLoading(true);
        }
    }
    return (
        <article className="list-item">
            <p onClick={handleNav}>{props.trackingNumber}</p>
            <p>Sender: {props.sender}</p>
            <p>Recipient: {props.recipient}</p>
            <p>Status: {props.currentStatus}</p>
            <div>
                {props.allowedTransitions.map((status) => (
                    <button key={status} onClick={() => handleChangeStatus(status)}>
                        Change to {status}
                    </button>
                ))}
            </div>
            <p>Date Created: {new Date(props.dateCreated).toLocaleDateString()}</p>
        </article>
    );
}