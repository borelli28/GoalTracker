'use client';

import React from 'react';
import dynamic from 'next/dynamic';

const CalendarHeatmap = dynamic(() => import('react-calendar-heatmap'), { 
  ssr: false,
  loading: () => <p>Loading...</p>
});

export default function ProgressGrid({ progress = [] }) {
  const [isClient, setIsClient] = React.useState(false);

  React.useEffect(() => {
    setIsClient(true);
  }, []);

  if (!isClient) {
    return null;
  }

  const values = progress.map(p => ({
    date: p.date,
    count: p.completed ? 1 : 0
  }));

  const today = new Date();
  const startDate = new Date(today.getFullYear(), today.getMonth() - 2, 1);
  const endDate = today;

  return (
    <div>
      <CalendarHeatmap
        startDate={startDate}
        endDate={endDate}
        values={values}
        classForValue={(value) => {
          if (!value) {
            return 'color-empty';
          }
          return `color-scale-${value.count}`;
        }}
        titleForValue={(value) => {
          if (!value) {
            return 'No data';
          }
          return `Date: ${value.date}, Completed: ${value.count === 1 ? 'Yes' : 'No'}`;
        }}
      />
      <style jsx global>{`
        .react-calendar-heatmap .color-empty { fill: #eeeeee; }
        .react-calendar-heatmap .color-scale-0 { fill: #d6e685; }
        .react-calendar-heatmap .color-scale-1 { fill: #44a340; }
      `}</style>
    </div>
  );
}
