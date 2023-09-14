import axios from '../custom-axios/axios';

const InventoryManagementService = {

    fetchItems: () => {
        return axios.get("https://localhost:7213/api/items")
    }

}

export default InventoryManagementService;