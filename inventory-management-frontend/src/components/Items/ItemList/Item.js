import React, {Component} from 'react';

class Item extends Component {

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <tr>
                <td>{this.props.item.name}</td>
                <td>{this.props.item.category.name}</td>
                <td>${this.props.item.price}</td>
                <td>{this.props.item.quantity}</td>
            </tr>
        );
    }

}

export default Item;