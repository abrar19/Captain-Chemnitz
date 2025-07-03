import React, {useState} from 'react';
import Card from "react-bootstrap/Card";
import {APIEndpoints} from "../../constants/APIEndpoints.js";
import {jwtDecode} from "jwt-decode";
import {Table} from "react-bootstrap";
import Form from "react-bootstrap/Form";



function AdminUserDashboard() {


    const isTokenExpired = (token) => {
        try {
            const { exp } = jwtDecode(token);
            return Date.now() >= exp * 1000;
        } catch {
            return true;
        }
    };

    const isAdmin = () => {
        const token = JSON.parse(localStorage.getItem('token'))?.token;
        if (!token || isTokenExpired(token)) {
            return false;
        }
        try {
            const decodedToken = jwtDecode(token);
            return decodedToken.role === 'Admin';

        } catch (error) {
            console.error('Error decoding token:', error);
            return false;
        }
    };


    const [users, setUsers] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [showInactiveUsers, setShowInactiveUsers] = useState(false);





    const fetchUsers = async (isInactive) => {
        try {
            setIsLoading(true);
            const tokenData = JSON.parse(localStorage.getItem('token'));
            const token = tokenData?.token;

            var endPoint = isInactive? APIEndpoints.getAllInactiveUsers: APIEndpoints.getAllUsers;
            const response = await fetch(endPoint, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }

            const data = await response.json();
            setUsers(data);
        } catch (error) {
            console.error('Error fetching users:', error);
        } finally {
            setIsLoading(false);
        }
    }
    useState(() => {
        if (!isAdmin()) {
            alert("You are not authorized to view this page");
            window.location.href = '/';
            localStorage.removeItem('token');
            return;
        }

        fetchUsers(false);
    }, []);

    function  handleTypeChange(e) {
        setShowInactiveUsers(e.target.checked);
        console.log(showInactiveUsers);
        fetchUsers(e.target.checked);
    }

    return(
        <div className="dashboard">
            <aside className="sidebar">
                <h2 className="sidebar-title text-white">Admin Panel</h2>
                <nav>
                    <ul className="sidebar-nav">
                        <li><a href="/admin">Dashboard</a></li>
                        <li><a href="/admin/users">Users</a></li>
                    </ul>
                </nav>
            </aside>


            <main className="main-content">
              <h2>Users</h2>
                {isLoading ? (
                    <div className="loading">
                    <p>Loading users...</p>
                    </div>
                ) : (
                    <div className="user-list">

                        <Form>
                            <Form.Check // prettier-ignore
                                type="switch"
                                id="custom-switch"
                                checked={showInactiveUsers}
                                onChange={handleTypeChange}
                                label="Show Only Inactive Users"
                            />
                        </Form>

                        <Table striped bordered hover>
                            <thead>
                            <tr>
                                <th>#</th>
                                <th>First Name</th>
                                <th>Last Name</th>
                                <th>Email</th>
                            </tr>
                            </thead>
                            <tbody>
                            {/*<tr>*/}
                            {/*    <td>1</td>*/}
                            {/*    <td>Mark</td>*/}
                            {/*    <td>Otto</td>*/}
                            {/*    <td>@mdo</td>*/}
                            {/*</tr>*/}
                            {/*<tr>*/}
                            {/*    <td>2</td>*/}
                            {/*    <td>Jacob</td>*/}
                            {/*    <td>Thornton</td>*/}
                            {/*    <td>@fat</td>*/}
                            {/*</tr>*/}
                            {/*<tr>*/}
                            {/*    <td>3</td>*/}
                            {/*    <td colSpan={2}>Larry the Bird</td>*/}
                            {/*    <td>@twitter</td>*/}
                            {/*</tr>*/}
                            {users.map((user, index) => (
                                <tr key={index}>
                                    <td>{index + 1}</td>
                                    <td>{user.firstName}</td>
                                    <td>{user.lastName}</td>
                                    <td>{user.email } {user.emailVerified && (
                                        <span className="text-success">✅</span>
                                    )}</td>
                                </tr>
                            ))}
                            </tbody>
                        </Table>

                    </div>
                )}
            </main>
        </div>

    )
}

export default AdminUserDashboard;