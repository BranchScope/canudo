'use client'
import '../app/globals.css';
import React, { useState } from 'react';

const LoginPage = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = (e) => {
    e.preventDefault();
    // Your login authentication logic goes here
    console.log('Logged in with username:', username);
  };

  return (
    <div className="flex items-center justify-center h-screen">
      <form className="bg-gray-950 shadow-md rounded px-10 pt-8 pb-10 mb-4" onSubmit={handleLogin}>
        <h2 className="text-2xl font-bold mb-4 text-white justify-center">FantaCanudo</h2>
        <div className="mb-4">
          <label htmlFor="username" className="block text-white-950 text-sm font-bold mb-2">Username:</label>
          <input
            type="text"
            id="username"
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </div>
        <div className="mb-6">
          <label htmlFor="password" className="block text-white-950 text-sm font-bold mb-2">Password:</label>
          <input
            type="password"
            id="password"
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <button
          type="submit"
          className="bg-blue-950 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
        >
          Accedi
        </button>
        <button
          type="submit"
          className="bg-blue-950 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
        >
          Registrati
        </button>
      </form>
    </div>
  );
};

export default LoginPage;
