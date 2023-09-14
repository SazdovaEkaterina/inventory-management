import React, { Component } from "react";
import Item from "./Item";

class Items extends Component {

    constructor(props) {
        super(props);
    }

    render(){
        return (
            <div className={"container mm-4 mt-5"}>
                <h1>Item Inventory</h1>
                <div className={"table-responsive"}>
                    <div className={"table-responsive"}>
                        <table className={"table table-striped"}>
                            <thead>
                                <tr>
                                    <th scope={"col"}>Name</th>
                                    <th scope={"col"}>Category</th>
                                    <th scope={"col"}>Price</th>
                                    <th scope={"col"}>Quantity</th>
                                </tr>
                            </thead>
                            <tbody>
                                {this.props.items.map((item) => (
                                    <Item key={item.Id} item={item}/>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        );
    }
}

export default Items;