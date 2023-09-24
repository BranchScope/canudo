'use client'
import '../app/globals.css';
import React, { useState } from 'react';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from '@fortawesome/fontawesome-svg-core'
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons'
library.add(faEye, faEyeSlash)

const LoginPage = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [visible, setVisible] = useState(false);

  const handleLogin = (e) => {
    e.preventDefault();
    // Your login authentication logic goes here
    console.log('Logged in with username:', username);
  };

  return (
    <div className="bg-gray-950 flex items-center justify-center h-screen">
      <form className="bg-slate-900 shadow-md rounded-[18px] px-16 pt-10 pb-10 mb-10" onSubmit={handleLogin}>
        <h2 className="text-3xl font-bold mb-1 text-white justify-center">FantaClasse</h2>
        <h2 className="text-md font-italic mb-4 text-gray-400 justify-center">Il primo FantaClasse digitale in italia!</h2>
        <div className="mb-4">
          <label htmlFor="username" className="block text-white-950 text-md font-bold mb-2">Username:</label>
          <input
            type="text"
            id="username"
            className="shadow appearance-none border border-gray-500 focus:border-white rounded-lg w-full py-2 px-3 bg-slate-900 text-white-700 leading-tight focus:outline-none focus:shadow-outline"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </div>
        <div className="mb-6 relative">
          <label htmlFor="password" className="block text-white-950 text-md font-bold mb-2">Password:</label>
            <div className='relative'>
              <input
              type={visible ? "text" : "password"}
              id="password"
              className="shadow appearance-none border border-gray-500 focus:border-white rounded-lg w-full py-2 px-3 bg-slate-900 text-white-700 leading-tight focus:outline-none focus:shadow-outline"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <span
              className="absolute inset-y-0 right-0 flex items-center pr-3"
              onClick={() => setVisible(!visible)}
            >
              <FontAwesomeIcon
                icon={visible ? faEyeSlash : faEye}
                className="text-gray-500 cursor-pointer"
              />
            </span>
          </div>
        </div>
        <div className="flex items-center justify-center space-x-4">
          <button
            type="submit"
            className="bg-blue-950 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded-md focus:outline-none focus:shadow-outline"
          >
            Accedi
          </button>
          <button
            type="submit"
            className="bg-blue-950 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded-md focus:outline-none focus:shadow-outline"
          >
            Registrati
          </button>
        </div>
      </form>
    </div>
  );
};

export default LoginPage;
