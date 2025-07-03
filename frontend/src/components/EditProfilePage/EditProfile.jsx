import React, { useEffect, useState } from "react";
import "./EditProfile.css";
import {APIEndpoints} from "../../constants/APIEndpoints.js";
import {Tab, Tabs} from "react-bootstrap";

const EditProfile = () => {
  const [formData, setFormData] = useState({
    FirstName: "",
    LastName: "",
    Email: ""
  });

    const [formDataPassword, setFormDataPassword] = useState({
    currentPassword: "",
    password: ""
    });


  const [message, setMessage] = useState("");
  const [passwordMessage, setPasswordMessage] = useState("");

  const { userId, token } = JSON.parse(localStorage.getItem("token"));


  const [key, setKey] = useState('profile');

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const res = await fetch(APIEndpoints.getUserProfile, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) throw new Error("Failed to fetch user info");

        const data = await res.json();
        setFormData((prev) => ({ ...prev, FirstName: data.firstName, LastName: data.lastName, Email: data.email }));
      } catch (err) {
        setMessage(err.message);
      }
    };

    fetchUser();
  }, [userId, token]);

  const handleChange = (e) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

    const handleChangePassword = (e) => {
    setFormDataPassword((prev) => ({
        ...prev,
        [e.target.name]: e.target.value,
    }));
    };


  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch(APIEndpoints.updateUserProfile , {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({

        "firstName": formData.FirstName,
          "lastName": formData.LastName,

        }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Failed to update profile");
      }

      setMessage("Profile updated successfully.");
    } catch (error) {
      setMessage(error.message);
    }
  };



    const handleSubmitPassword = async (e) => {
    e.preventDefault();
      try {
        const response = await fetch(APIEndpoints.updateUserPassword , {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({

            "oldPassword": formDataPassword.currentPassword,
            "newPassword": formDataPassword.password,

          }),
        });

        if (!response.ok) {
          const errorText = await response.json()
            .then(data => data.message)

          setPasswordMessage(errorText || "Failed to update password");
          throw new Error(errorText || "Failed to update password");

        }


        setPasswordMessage("Password updated successfully.");
      } catch (error) {
        setPasswordMessage(error.message);
      }
    }



  return (
    <div className="edit-profile-container">
      <h2>My Profile</h2>
    <Tabs
    activeKey={key}
    onSelect={(k) => setKey(k)}
    id="controlled-tab-example"
    className="mb-3"
    >
      <Tab title="Profile" eventKey="profile">
        <form onSubmit={handleSubmit} className="edit-profile-form">
          <label>
            First Name:
            <input
                type="text"
                name="FirstName"
                value={formData.FirstName}
                onChange={handleChange}
            />
          </label>
          <label>
            Last Name:
            <input
                type="text"
                name="LastName"
                value={formData.LastName}
                onChange={handleChange}
            />
          </label>
          <label>
            Email:
            <input
                type="email"
                name="Email"
                value={formData.Email}
                disabled
            />
          </label>
          <button type="submit">Update Profile</button>
        </form>
        {message && <p className="message">{message}</p>}

        <br />

      </Tab>

        <Tab title="Password" eventKey="password">

          <form onSubmit={handleSubmitPassword} className="edit-profile-form">
            <label>
              Current Password:
              <input
                  type="password"
                  name="currentPassword"
                  value={formDataPassword.currentPassword}
                  onChange={handleChangePassword}
              />
            </label>
            <label>
              New Password:
              <input
                  type="password"
                  name="password"
                  value={formDataPassword.password}
                  onChange={handleChangePassword}
              />
            </label>
            <button type="submit">Update Password</button>
          </form>
          {passwordMessage && <p className="message">{passwordMessage}</p>}
        </Tab>
    </Tabs>
    </div>
  );
};

export default EditProfile;
