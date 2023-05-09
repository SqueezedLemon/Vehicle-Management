const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("ReceiveAdminNotification", (senderName, requestId, notificationType,) => {
    // Handle the notification
    var notificationContainer = document.getElementById("alertsDropdownBody");
    var newNotification = document.createElement("a");
    newNotification.classList.add("dropdown-item", "d-flex", "align-items-center");
    if (notificationType == "Needs Approval") {
        newNotification.href = `/Home/ApproveRequest/${requestId}`;
    } else {
        newNotification.href = `/Home/ViewRequests`;
    }

    // Add notification icon*@
    var iconContainer = document.createElement("div");
    iconContainer.classList.add("mr-3");
    var iconCircle = document.createElement("div");
    iconCircle.classList.add("icon-circle", "bg-primary");
    var icon = document.createElement("i");
    icon.classList.add("fas", "fa-file-alt", "text-white");
    iconCircle.appendChild(icon);
    iconContainer.appendChild(iconCircle);
    newNotification.appendChild(iconContainer);

    //Add notification details
    var notificationDetails = document.createElement("div");
    var dateText = document.createTextNode(new Date().toLocaleString());
    notificationDetails.classList.add("flex-grow-1");
    var dateElement = document.createElement("div");
    dateElement.classList.add("small", "text-gray-500");
    dateElement.appendChild(dateText);
    var senderNameText = document.createTextNode("Request by " + senderName + " needs approval");
    if (notificationType == "Request Completed") {
        senderNameText = document.createTextNode("Request by "+ senderName + " has completed a request.");
    }
    var senderNameElement = document.createElement("span");
    senderNameElement.classList.add("font-weight-bold");
    senderNameElement.appendChild(senderNameText);
    notificationDetails.appendChild(dateElement);
    notificationDetails.appendChild(senderNameElement);
    newNotification.appendChild(notificationDetails);

    notificationContainer.insertBefore(newNotification, notificationContainer.firstChild);

    console.log(`Received notification: ${notificationType}`);
});

document.addEventListener("DOMContentLoaded", function () {
    connection.start()
        .then(() => console.log("SignalR connection established."))
        .catch((error) => console.error(error));
});