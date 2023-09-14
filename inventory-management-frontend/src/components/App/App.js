import './App.css';
import React, {Component} from "react";
import InventoryManagementService from '../../repository/InventoryManagementRepository';
import Header from '../Header/Header';
import Items from '../Items/ItemList/Items'

class App extends Component {

  constructor(props){
    super(props);
    this.state = {
      items: []
    }
  }

  render() {
    return (
      <div className="App">
        <Header/>
        <main>
          <div className="container">
            <Items items={this.state.items}/>
          </div>
        </main>
      </div>
    );
  }

  componentDidMount(){
    this.loadItems();
  }

  loadItems = () => {
    InventoryManagementService.fetchItems()
      .then((data) => {
        this.setState({
          items: data.data
        })
      })
  }
}

export default App;
