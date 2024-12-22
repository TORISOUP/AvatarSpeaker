module.exports = {
    name: 'AvatarSpeakerPlugin', 
    uid: 'net.torisoup', 
    version: '0.0.1', 
    author: 'TORISOUP', 
    url: 'https://github.com/TORISOUP/AvatarSpeaker', 
    permissions: ['comments'],
    subscribe(type, ...args) {
        for (const x of args[0].comments) {

            const data = {text: x.data.comment.toString()};

            fetch('http://127.0.0.1:21012/api/v1/speakers/current/speak_current_parameters', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data) 
            })
        }
    }
}