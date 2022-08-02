const Axios = require("axios").default;

export default class AxiosServices {
  //Post Services
  post(url, data, IsRequired = false, Header) {
    console.log("Url : ", url, " Data : ", data);
    return Axios.post(url, data, IsRequired && Header);
  }

  //Get Services
  get(url, IsRequired = false, Header) {
    return Axios.get(url, IsRequired && Header);
  }

  //Put Services
  put(url, data, IsRequired = false, Header) {
    console.log("Url : ", url, " Data : ", data);
    return Axios.put(url, data, IsRequired && Header);
  }

  //Delete Services
  delete(url, data, IsRequired = false, Header) {
    console.log("Url : ", url, " Data : ", data);
    return Axios.delete(url, data, IsRequired && Header);
  }
}
