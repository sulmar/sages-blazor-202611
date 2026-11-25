// Moduł JS izolowany dla komponentu CameraCapture.razor.

export async function start(video, dotNetRef) {
    stop(video);

    if (!navigator.mediaDevices?.getUserMedia) {
        throw new Error("Kamera wymaga HTTPS lub localhost.");
    }

    const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: { ideal: "environment" } },
        audio: false
    });

    // Użytkownik mógł opuścić stronę podczas pytania o zgodę.
    if (!video.isConnected) {
        stream.getTracks().forEach(track => track.stop());
        return;
    }

    // JS -> .NET: informujemy komponent, gdy kamera sama przestanie nadawać.
    stream.getVideoTracks().forEach(track => {
        track.onended = () => dotNetRef.invokeMethodAsync("OnCameraEnded");
    });

    video.srcObject = stream;
    await video.play();
}

export function stop(video) {
    // Zatrzymanie przez nas nie jest zdarzeniem "ended" - komponent wie o nim sam.
    video.srcObject?.getTracks().forEach(track => {
        track.onended = null;
        track.stop();
    });
    video.srcObject = null;
}

export async function capture(video) {
    if (!video.videoWidth || !video.videoHeight) {
        throw new Error("Poczekaj na obraz z kamery.");
    }

    const canvas = document.createElement("canvas");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext("2d").drawImage(video, 0, 0);

    // Zwracamy Blob, a nie tekst base64: po stronie .NET typ IJSStreamReference
    // sprawia, że dane płyną porcjami zamiast w jednym dużym komunikacie.
    return await new Promise(resolve => canvas.toBlob(resolve, "image/jpeg", 0.9));
}
