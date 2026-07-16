// Handles push messages that arrive while no tab has the app in the foreground.
// Config values below are the same public client identifiers baked into
// lib/firebase_options.dart — not secrets (see Firebase's own docs on this).
importScripts('https://www.gstatic.com/firebasejs/10.14.1/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.14.1/firebase-messaging-compat.js');

firebase.initializeApp({
  apiKey: 'AIzaSyCkz2DPoFyZ1FK3olJ4kbIF6Wij1EcTdQ4',
  appId: '1:94600500115:web:fb14c644d03f858bb94e66',
  messagingSenderId: '94600500115',
  projectId: 'you-g-f0698',
  authDomain: 'you-g-f0698.firebaseapp.com',
  storageBucket: 'you-g-f0698.firebasestorage.app',
});

firebase.messaging();
