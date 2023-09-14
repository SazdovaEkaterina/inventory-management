import React, {Component} from "react";

class Header extends Component{

    render() {
        return (
            <header>
                <nav className="navbar navbar-expand-md navbar-light navbar-fixed bg-light">
                    <a clasName="navbar-brand" href="#">InventoryManagement</a>
                    <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarCollapse"
                            aria-controls="navbarCollapse" aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="collapse navbar-collapse" id="navbarCollapse">
                        <ul className="navbar-nav mr-auto">
                            <li className="nav-item active">
                                <a className="nav-link" href="#">Link1</a>
                            </li>
                            <li className="nav-item active">
                                <a className="nav-link" href="#">Link2</a>
                            </li>
                        </ul>
                    </div>
                </nav>
            </header>
        );
    }
}

export default Header;