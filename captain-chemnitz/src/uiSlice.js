import { createSlice } from '@reduxjs/toolkit';

const uiSlice = createSlice({
  name: 'ui',
  initialState: {
    searchTerm: '',
    features: [],  // full list of features (GeoJSON)
    selectedFeature: null,
    mapReady: false
  },
  reducers: {
    setSearchTerm: (state, action) => {
      state.searchTerm = action.payload;
    },
    setFeatures: (state, action) => {
      state.features = action.payload;
    },
    setSelectedFeature: (state, action) => {
      state.selectedFeature = action.payload;
    },
    setMapReady: (state, action) => {
      state.mapReady = action.payload;
    }
  }
});

export const { setSearchTerm, setFeatures, setSelectedFeature, setMapReady } = uiSlice.actions;

export default uiSlice.reducer;
