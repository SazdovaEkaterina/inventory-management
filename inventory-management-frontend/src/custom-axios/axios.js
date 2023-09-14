import axios from "axios";

const instance = axios.create({
    baseUrl: '',
    headers: {
        'Access-Control-Allow-Origin' : '*'
    }
})

export default instance;