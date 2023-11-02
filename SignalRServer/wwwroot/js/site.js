// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var myToken = $("#myToken").val();
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/learningHub", { accessTokenFactory: () => myToken })
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.on("ReceiveMessage", message => {
    $("#signalr-message-panel").prepend($("<div />").text(message));
});

$("#btn-broadcast").click(() => {
    var message = $("#broadcast").val();
    if (message.includes(';')) {
        var messages = message.split(';');
        var subject = new signalR.Subject();
        connection.send("BroadcastStream", subject).catch(err => console.error(err.toString()));
        for (var i = 0; i < messages.length; i++) {
            subject.next(messages[i]);
        }
        subject.complete();
    }
    else {
        connection.invoke("BroadcastMessage", message).catch(err => console.error(err.toString()));
    }
});

$("#btn-other-message").click(() => {
    var message = $("#other-message").val();
    connection.invoke("SendToOthers", message).catch(err => console.error(err.toString()));
});

$("#btn-self-message").click(() => {
    var message = $("#self-message").val();
    connection.invoke("SendToCallers", message).catch(err => console.error(err.toString()));
});

$("#btn-indivisual-message").click(() => {
    var message = $("#indivisual-message").val();
    var connectionId = $("#connection-id").val();
    connection.invoke("SendToIndivisual", connectionId, message).catch(err => console.error(err.toString()));
});

$("#btn-group-message").click(() => {
    var message = $("#group-message").val();
    var groupName = $("#group-for-message").val();
    connection.invoke("SendToGroup", groupName, message).catch(err => console.error(err.toString()));
});

$("#btn-add-user-for-group").click(() => {
    var groupName = $("#add-user-for-group").val();
    connection.invoke("AddUserToGroup", groupName).catch(err => console.error(err.toString()));
});

$("#btn-remove-user-from-group").click(() => {
    var groupName = $("#remove-user-from-group").val();
    connection.invoke("RemoveUserFromGroup", groupName).catch(err => console.error(err.toString()));
});

$("#btn-trigger-stream").click(() => {
    var numberOfJobs = parseInt($("#number-of-jobs").val(), 10);
    connection.stream("TriggerStream", numberOfJobs).subscribe({
        next: (message) => $('#signalr-message-panel').prepend($('<div />').text(message))
    });
});

async function start() {
    try {
        await connection.start();
        console.log('connected');
    }
    catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
}

connection.onclose(async () => {
    await start();
});

start();