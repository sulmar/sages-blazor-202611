export function connect(dotNetRef) {
    const channel = new BroadcastChannel("counter");

    channel.onmessage = event => {
        dotNetRef.invokeMethodAsync("ReceiveMessage", event.data)
            .catch(console.error);
    };

    return {
        send(value) {
            channel.postMessage(value);
        },
        close() {
            channel.onmessage = null;
            channel.close();
        }
    };
}