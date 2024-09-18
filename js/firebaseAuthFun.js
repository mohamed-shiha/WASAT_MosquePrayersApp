import {
    signInWithEmailAndPassword,
    createUserWithEmailAndPassword,
    signOut,
    onAuthStateChanged,
    getAuth
} from "https://www.gstatic.com/firebasejs/10.13.1/firebase-auth.js";
import {
    getFirestore,
    doc,
    getDoc,
    getDocs,
    collection,
    setDoc
} from "https://www.gstatic.com/firebasejs/10.13.1/firebase-firestore.js";

let auth;
let db;

export function initializeFirebaseAuth(app) {
    auth = getAuth(app);
    db = getFirestore(app);

    window.firebaseAuth = {
        signIn: async function (email, password) {
            try {
                const userCredential = await signInWithEmailAndPassword(auth, email, password);

                const IdToken = await userCredential.user.getIdToken();
                return {
                    Token: IdToken, Email: email
                } 
            } catch (error) {
                throw new Error(error.message);
            }
        },

        createUserRole: async function (uid, role = 'user') {
            try {
                const docRef = doc(db, 'user_roles', uid);
                await setDoc(docRef, { role: role });
                console.log("User role document created successfully");
            } catch (error) {
                console.error("Error creating user role document:", error);
                throw error;
            }
        },
        signOut: async function () {
            return signOut(auth);
        },

        getCurrentUser: function () {
            return new Promise((resolve) => {
                const unsubscribe = onAuthStateChanged(auth, (user) => {
                    unsubscribe();
                    resolve(user);
                });
            });
        },

        register: async function (email, password) {
            try {
                const userCredential = await createUserWithEmailAndPassword(auth, email, password);
                await this.createUserRole(userCredential.user.uid);
                return await userCredential.user.getIdToken();
            } catch (error) {
                throw new Error(error.message);
            }
        },

        getIdToken: function () {
            return new Promise((resolve) => {
                const unsubscribe = onAuthStateChanged(auth, (user) => {
                    unsubscribe();
                    if (user) {
                        user.getIdToken().then(resolve);
                    } else {
                        resolve(null);
                    }
                });
            });
        },

        getUserRole: async function () {
            const user = auth.currentUser;
            if (user) {
                try {
                    console.log("Current user UID:", user.uid);
                    const docRef = doc(db, 'user_roles', user.uid);
                    console.log("DocRef path:", docRef.path);

                    // Check if the collection exists
                    const collectionRef = collection(db, 'user_roles');
                    const collectionSnapshot = await getDocs(collectionRef);
                    console.log("Collection exists:", !collectionSnapshot.empty);
                    console.log("Number of documents in collection:", collectionSnapshot.size);

                    const docSnap = await getDoc(docRef);
                    if (docSnap.exists()) {
                        console.log("Document data:", docSnap.data());
                        return docSnap.data().role;
                    } else {
                        console.log("No such document!");
                        return 'user'; // Default role if not set
                    }
                } catch (error) {
                    console.error("Error getting user role:", error);
                    throw error;
                }
            } else {
                console.log("No user is signed in.");
                return null;
            }
        }
    };
}