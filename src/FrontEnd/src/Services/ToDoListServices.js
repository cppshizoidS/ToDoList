import AxiosServices from './AxiosServices'
import Configuration from '../configuration/Configuration'

const axiosServices = new AxiosServices()

export default class ToDoListServices {
  InsertNote(data) {
    return axiosServices.post(Configuration.InsertNote, data, false)
  }
  GetNote(data) {
    return axiosServices.post(Configuration.GetNote, data, false)
  }
  GetNoteById(data) {
    let Url = Configuration.GetNoteById + data
    return axiosServices.get(Url, false)
  }
  UpdateNote(data) {
    return axiosServices.put(Configuration.UpdateNote, data, false)
  }
  DeleteNote(data) {
    let Url = Configuration.DeleteNote + data
    return axiosServices.delete(Url, false)
  }
}
