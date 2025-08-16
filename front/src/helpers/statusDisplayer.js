export const statusDisplayer = (statusNumber) => {
    switch (statusNumber) {
        case 0:
            return "Created";
        case 1:
            return "Sent";
        case 2:
            return "Accepted";
        case 3:
            return "Returned";
        case 4:
            return "Canceled";
        default:
            return "Unknown Status";
    }
}