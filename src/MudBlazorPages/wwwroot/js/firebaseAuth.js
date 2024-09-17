window.firebaseAuth = {
    signIn: function (email, password) {
        return firebase.auth().signInWithEmailAndPassword(email, password)
            .then((userCredential) => {
                return userCredential.user.getIdToken();
            })
            .catch((error) => {
                // Instead of returning null, throw an error to be caught by the C# code
                throw new Error(error.message);
            });
    },
    signOut: function () {
        return firebase.auth().signOut();
    },
    getCurrentUser: function () {
        return new Promise((resolve, reject) => {
            firebase.auth().onAuthStateChanged((user) => {
                if (user) {
                    resolve(user);
                } else {
                    resolve(null);
                }
            });
        });
    },
    register: function (email, password) {
        return firebase.auth().createUserWithEmailAndPassword(email, password)
            .then((userCredential) => {
                return userCredential.user.getIdToken();
            });
    },
    getIdToken: function () {
        return new Promise((resolve, reject) => {
            const unsubscribe = firebase.auth().onAuthStateChanged((user) => {
                unsubscribe();
                if (user) {
                    user.getIdToken().then(resolve, reject);
                } else {
                    resolve(null);
                }
            });
        });
    }
};
