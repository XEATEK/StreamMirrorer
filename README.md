# Twitch Local Archive (Project Name TBD)

![Status](https://img.shields.io/badge/status-in%20development-orange)

A robust, self-hosted application designed to automatically archive Twitch livestreams to a local server and provide a simple web interface to manage and view them.

---

## About The Project

Have you ever missed a livestream from your favorite creator, only to find the VOD was muted? This project aims to solve that problem by creating a reliable, "set-and-forget" solution for archiving Twitch streams on your own hardware.

The goal is to build a robust application that monitors specified Twitch channels, automatically records their streams from the moment they go live, and saves them directly to your local server. A user-friendly web interface will then allow you to easily browse, watch, and manage your personal archive.

This project is currently in the planning and development phase. The core focus is on building a stable and scalable system from the ground up.

## Core Features (Planned)

*   **Automated Recording**: Automatically detects when a monitored Twitch channel goes live and begins recording immediately.
*   **Local First Storage**: All video files are saved directly to your own server, giving you full control over your data.
*   **Web Interface**: A clean UI to view and manage your archived videos.
    *   Browse a library of all saved recordings.
    *   View metadata like stream title, channel, date, and duration.
    *   Play videos directly in your browser.
    *   Delete archives to manage disk space.
*   **Robust & Resilient**: Designed to handle stream interruptions and API changes gracefully, ensuring the most complete archive possible.
*   **Scalable Storage**: Built with an object storage architecture (like S3) to handle large video files efficiently and reliably.

## How It Works (Technical Approach)

To ensure stability and avoid reinventing the wheel, this application will be built on top of proven, industry-standard tools:

1.  **Stream Discovery**: The application will use `Streamlink`, a powerful and actively maintained CLI tool, to handle the complexities of authenticating with the Twitch API and retrieving the live HLS stream playlist (`.m3u8`). This abstracts away the most fragile part of the process, making the application less prone to breaking when Twitch updates its API.

2.  **Video Processing**: The actual recording and stitching of video segments will be handled by `FFmpeg`, the gold standard for video manipulation. To ensure clean and maintainable code, a C# wrapper like `Xabe.FFmpeg` will be used to programmatically control FFmpeg, rather than constructing fragile command-line strings.

3.  **Storage Architecture**: Video files are large and require a robust storage solution. Instead of writing directly to the filesystem (which can be prone to corruption), this project will use a self-hosted, S3-compatible object storage solution like **MinIO** or **GarageHQ**.
    *   **Metadata** (like the stream title, date, and a key for the video file) will be stored in a lightweight database.
    *   **The large video file (the "object")** will be stored in the object storage.
    *   This separation ensures that the database remains fast and that video file operations are scalable and resilient.

## Project Vision

The ultimate vision is to create a polished, open-source application that anyone can easily deploy on their home server or NAS. It should be a reliable tool for content creators, researchers, or any fan who wants to maintain a personal, permanent archive of Twitch content.

## Current Status

This project is in it's **development stage**. The core architecture has been designed, but there is no functional release available yet.

## FAQ
Q: How do I get my twitch OAuth token?
A: Open your console on any twitch page and paste the following code. This returns your OAuth token.
```javascript
document.cookie.split("; ").find(item=>item.startsWith("auth-token="))?.split("=")[1]
```
