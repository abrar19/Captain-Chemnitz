import React, { useEffect, useState } from "react";
import "./EditProfile.css";

const EditProfile = () => {
  const [formData, setFormData] = useState({
    username: "",
    password: "",
  });
  const [message, setMessage] = useState("");

  const { userId, token } = JSON.parse(localStorage.getItem("token"));

  useEffect(() => {
    // Fetch current user info to show current username
    const fetchUser = async () => {
      try {
        const res = await fetch(`http://localhost:5029/api/user/${userId}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) throw new Error("Failed to fetch user info");

        const data = await res.json();
        setFormData((prev) => ({ ...prev, username: data.username }));
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

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch(`http://localhost:5029/api/user/${userId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(formData),
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

  return (
    <div className="edit-profile-container">
      <h2>Edit Profile</h2>
      <form onSubmit={handleSubmit} className="edit-profile-form">
        <label>
          Username:
          <input
            type="text"
            name="username"
            value={formData.username}
            onChange={handleChange}
          />
        </label>
        <label>
          New Password:
          <input
            type="password"
            name="password"
            placeholder="Leave blank to keep current"
            value={formData.password}
            onChange={handleChange}
          />
        </label>
        <button type="submit">Update Profile</button>
      </form>
      {message && <p className="message">{message}</p>}
    </div>
  );
};

export default EditProfile;
