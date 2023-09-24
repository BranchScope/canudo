'use client'
import '../app/globals.css';
import React from 'react';

const DashboardPage = () => {

  // get cookies from browser and check the session token through the API

  return (
      <div className="bg-gray-950 flex items-center justify-center h-screen">
        <div className="text-center">
          <h1 className="text-4xl font-bold mb-4">FantaClasse</h1>
          <p className="text-lg">Il primo FantaClasse digitale in Italia!<br />Qualcosa bolle in pentola... e non sono patate :)</p>
        </div>
      </div>
  );
}

export default DashboardPage;