# 🩺 XI-Surg - VR Medical Training System

**Meta Horizon Developer Competition Submission**

XI-Surg is an immersive VR medical training platform designed for Meta Quest devices. It provides medical students and professionals with realistic, hands-on training experiences for various surgical procedures in a safe, controlled virtual environment.

[![Unity Version](https://img.shields.io/badge/Unity-6000.2.4f1-blue.svg)](https://unity.com/)
[![XR Platform](https://img.shields.io/badge/XR-Meta%20Quest-green.svg)](https://www.meta.com/quest/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 🎯 Project Overview

XI-Surg revolutionizes medical education by leveraging VR technology to create realistic surgical training simulations. The platform offers step-by-step guided procedures, real-time feedback, and progress tracking to enhance learning outcomes.

### Key Features

- **🔐 Secure Authentication**: Firebase-based user authentication with session management
- **🎮 Multiple Training Modes**: 
  - **Training Mode**: Practice freely without guidance
  - **Guided Mode**: Step-by-step instructions and assistance
- **⚕️ Surgical Procedures**:
  - **Basic Surgeries**: Suturing, Injection
  - **Advanced Surgeries**: Laparoscopic Appendectomy, Intramedullary Tibial Nailing, Pacemaker Implantation
- **📊 Progress Tracking**: Real-time progress monitoring with detailed analytics
- **💓 Patient Monitoring**: Real-time vital signs and ECG waveform generation
- **🎨 Immersive UI**: World-space UI optimized for VR interaction
- **📱 Cross-Platform Dashboard**: Web-based dashboard for progress monitoring

## 🛠️ Technology Stack

- **Game Engine**: Unity 6000.2.4f1
- **XR Framework**: Meta XR SDK / Oculus Integration
- **XR Interaction**: Unity XR Interaction Toolkit
- **Authentication**: Firebase Authentication (REST API)
- **Database**: Firebase Realtime Database
- **Backend API**: Node.js/Express (for patient data)
- **Rendering**: Universal Render Pipeline (URP)

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **Unity Hub** and **Unity Editor 6000.2.4f1** or later
- **Meta Quest Device** (Quest 2, Quest 3, or Quest Pro)
- **Meta Quest Developer Account** (for device setup)
- **Android SDK** (for building APKs)
- **Git** (for cloning the repository)

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/moizrana/XI-Surg.git
cd XI-Surg
```

### 2. Open in Unity

1. Launch **Unity Hub**
2. Click **Add** and select the cloned `XI-Surg` folder
3. Unity will detect the project and show Unity 6000.2.4f1
4. Click **Open** to load the project

### 3. Configure XR Settings

1. Go to **Edit > Project Settings > XR Plug-in Management**
2. Install **Meta XR SDK** if not already installed
3. Enable **Meta XR SDK** for Android
4. Configure your Meta Quest device settings

### 4. Configure Firebase (Optional - for authentication)

1. Create a Firebase project at [Firebase Console](https://console.firebase.google.com/)
2. Enable **Authentication** (Email/Password)
3. Enable **Realtime Database**
4. Update `FirebaseAuthManager.cs` with your Firebase API key:
   ```csharp
   public string firebaseWebApiKey = "YOUR_FIREBASE_WEB_API_KEY";
   public string firebaseDatabaseUrl = "YOUR_FIREBASE_DATABASE_URL";
   ```

### 5. Build for Meta Quest

1. Connect your Meta Quest device via USB (enable Developer Mode)
2. Go to **File > Build Settings**
3. Select **Android** platform
4. Click **Switch Platform** (if needed)
5. Click **Build and Run** or **Build** to create APK

## 🎮 How to Use

### First Time Setup

1. **Launch the Application** on your Meta Quest device
2. **Login Screen**: 
   - Enter your registered email and password
   - Or click "Register" to open the registration website
3. **Selection Scene**: 
   - Choose your training mode (Training or Guided)
   - Select a surgery category (Basic or Advanced)
   - Choose a specific procedure

### Performing a Surgery

#### Suturing Procedure Example:

1. **Make Initial Incision**: Use the scalpel to make an incision on the marked line
2. **Clean the Wound**: 
   - Pick up antiseptic vial
   - Soak cotton swab in antiseptic
   - Apply swab to wound area (repeat until wound is clean)
3. **Complete Procedure**: Follow on-screen instructions to finish suturing

#### Injection Procedure Example:

1. **Pick Antiseptic**: Grab the antiseptic vial
2. **Soak Swab**: Dip cotton swab in antiseptic
3. **Swab Patient**: Apply swab to patient's skin
4. **Inject**: Perform the injection at the swabbed location

### Navigation

- **Grab Objects**: Use hand controllers to grab and interact with medical tools
- **UI Interaction**: Point and select using hand tracking or controllers
- **Menu Access**: Use the selection scene to switch between procedures

## 📁 Project Structure
