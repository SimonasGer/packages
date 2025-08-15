import { ListItem } from "./ListItem";
export const List = (props) => {
    return (
        <section>
            {props.packages.map((item) => (
                <ListItem
                    key={item.id}
                    id={item.id}
                    trackingNumber={item.trackingNumber}
                    sender={item.sender}
                    recipient={item.recipient}
                    currentStatus={item.currentStatus}
                    dateCreated={item.dateCreated}
                    allowedTransitions={item.allowedTransitions}
                />
            ))}
        </section>
    );
}