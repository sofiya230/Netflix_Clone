
import API_CONFIG from '../config/api.js';

const API_BASE_URL = API_CONFIG.BASE_URL;

const apiRequest = async (path, options = {}) => {
    const token = localStorage.getItem('auth_token');
    
    const config = {
        method: options.method || 'GET',
        headers: {
            'Content-Type': 'application/json',
            ...options.headers,
        },
        ...options,
    };

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    if (options.body) {
        config.body = JSON.stringify(options.body);
    }

    try {
        const res = await fetch(`${API_BASE_URL}${path}`, config);
        
        if (!res.ok) {
            const errorText = await res.text();
            
            let errorData;
            try {
                errorData = JSON.parse(errorText);
            } catch {
                errorData = { message: errorText || `HTTP ${res.status}: ${res.statusText}` };
            }
            
            const err = new Error(errorData.message || `HTTP ${res.status}: ${res.statusText}`);
            err.status = res.status;
            err.data = errorData;
            throw err;
        }

        const data = await res.json();
        return data;
    } catch (error) {
        throw error;
    }
};

function safeJson(text) {
    try { return JSON.parse(text); } catch { return null; }
}

export const authService = {
    register: (userData) => {
        const backendUserData = {
            Email: userData.email,
            FirstName: userData.firstName,
            LastName: userData.lastName,
            Password: userData.password,
            ConfirmPassword: userData.confirmPassword,
            DateOfBirth: userData.dateOfBirth,
            SubscriptionPlan: userData.subscriptionPlan || "Basic"
        };
        
        return apiRequest('/Auth/register', { method: 'POST', body: backendUserData });
    },

    login: async (credentials) => {
        const backendCredentials = {
            Email: credentials.email,
            Password: credentials.password
        };
        
        const resp = await apiRequest('/Auth/login', {
            method: 'POST',
            body: backendCredentials,
        });

        const token = resp?.token ?? resp?.Token ?? resp?.jwt ?? '';
        const user = resp?.user ?? resp?.User ?? null;
        const role = resp?.role ?? user?.role ?? null;

        if (token) localStorage.setItem('auth_token', token);
        if (user) localStorage.setItem('user_data', JSON.stringify(user));
        if (role) localStorage.setItem('user_role', role);

        return { 
            token, 
            user, 
            role, 
            raw: resp,
            isSuccessful: resp?.isSuccessful,
            requiresTwoFactor: resp?.requiresTwoFactor,
            message: resp?.message
        };
    },

    getCurrentUser: () => apiRequest('/Auth/me'),
    
    forgotPassword: (data) =>
        apiRequest('/Auth/forgot-password', { method: 'POST', body: data }),
    
    verifyResetCode: (data) =>
        apiRequest('/Auth/verify-reset-code', { method: 'POST', body: data }),
    
    resetPassword: (data) =>
        apiRequest('/Auth/reset-password', { method: 'POST', body: data }),

    enableTwoFactor: (data) =>
        apiRequest('/TwoFactorAuth/enable', { method: 'POST', body: data }),
    
    disableTwoFactor: (data) =>
        apiRequest('/TwoFactorAuth/disable', { method: 'POST', body: data }),
    
    getTwoFactorStatus: (email) =>
        apiRequest(`/TwoFactorAuth/status/${encodeURIComponent(email)}`),
    
    verifyTwoFactor: (data) =>
        apiRequest('/TwoFactorAuth/verify', { method: 'POST', body: data }),
    
    resendTwoFactorCode: (data) =>
        apiRequest('/TwoFactorAuth/resend-code', { method: 'POST', body: data }),
};


export const contentService = {

    getAllContents: () => apiRequest('/Contents'),
    getContentById: (id) => apiRequest(`/Contents/${id}`),
    createContent: (payload) =>
        apiRequest('/Contents', { method: 'POST', body: payload }),
    updateContent: (id, payload) =>
        apiRequest(`/Contents/${id}`, { method: 'PUT', body: payload }),
    deleteContent: (id) =>
        apiRequest(`/Contents/${id}`, { method: 'DELETE' }),


    getContentsByType: (type) => apiRequest(`/Contents/type/${encodeURIComponent(type)}`),
    getContentsByGenre: (genre) => apiRequest(`/Contents/genre/${encodeURIComponent(genre)}`),
    getByGenre: (genre) => apiRequest(`/Contents/genre/${encodeURIComponent(genre)}`),
    getTrendingContent: () => apiRequest('/Contents/trending'),
    getNewReleases: () => apiRequest('/Contents/new-releases'),
    searchContent: (query) => apiRequest(`/Contents/search?query=${encodeURIComponent(query)}`),


    getEpisodes: (contentId, seasonNumber = 1) =>
        apiRequest(`/Episodes/content/${contentId}/season/${seasonNumber}`),

    // “Similar” via genre or type (your API doesn’t have /similar)
    getSimilarContent: async (id) => {
        try {
            const item = await apiRequest(`/Contents/${id}`);
            if (!item) return [];
            if (item.genre) {
                const sameGenre = await apiRequest(`/Contents/genre/${encodeURIComponent(item.genre)}`);
                return (sameGenre || []).filter(c => c.id !== item.id);
            }
            const sameType = await apiRequest(`/Contents/type/${encodeURIComponent(item.contentType)}`);
            return (sameType || []).filter(c => c.id !== item.id);
        } catch {
            return [];
        }
    },
};


export const episodeService = {
    getEpisodeById: (id) => apiRequest(`/Episodes/${id}`),
    getEpisodes: (contentId, seasonNumber) =>
        apiRequest(`/Episodes/content/${contentId}/season/${seasonNumber}`),

    getEpisodesByContent: (contentId) =>
        apiRequest(`/Episodes/content/${contentId}`),

    getEpisodeByNumber: (contentId, seasonNumber, episodeNumber) =>
        apiRequest(`/Episodes/content/${contentId}/season/${seasonNumber}/episode/${episodeNumber}`),

    createEpisode: (payload) =>
        apiRequest('/Episodes', { method: 'POST', body: payload }),
    updateEpisode: (id, payload) =>
        apiRequest(`/Episodes/${id}`, { method: 'PUT', body: payload }),
    deleteEpisode: (id) =>
        apiRequest(`/Episodes/${id}`, { method: 'DELETE' }),
};


export const profileService = {
    getProfilesByUserId: (userId) => apiRequest(`/Profiles/user/${userId}`),
    createProfile: (userId, payload) =>
        apiRequest(`/Profiles/user/${userId}`, { method: 'POST', body: payload }),
    getProfileById: (id) => apiRequest(`/Profiles/${id}`),
    updateProfile: (id, payload) =>
        apiRequest(`/Profiles/${id}`, { method: 'PUT', body: payload }),
    deleteProfile: (id) => apiRequest(`/Profiles/${id}`, { method: 'DELETE' }),
};


export const myListService = {
    getMyList: (profileId) => apiRequest(`/MyList/profile/${profileId}`),
    addToMyList: (profileId, contentId) =>
        apiRequest('/MyList', { method: 'POST', body: { profileId, contentId } }),
    removeFromMyList: (profileId, contentId) =>
        apiRequest(`/MyList/content/${contentId}/profile/${profileId}`, { method: 'DELETE' }),
    isContentInMyList: (profileId, contentId) =>
        apiRequest(`/MyList/check/content/${contentId}/profile/${profileId}`),
};


export const userService = {
    getUser: (id) => apiRequest(`/Users/${id}`),
    updateUser: (id, payload) =>
        apiRequest(`/Users/${id}`, { method: 'PUT', body: payload }),
    changePassword: (id, payload) =>
        apiRequest(`/Users/${id}/change-password`, { method: 'PUT', body: payload }),
    changeSubscription: (id, payload) =>
        apiRequest(`/Users/${id}/change-subscription`, { method: 'PUT', body: payload }),
    deactivateUser: (id) => apiRequest(`/Users/${id}/deactivate`, { method: 'PUT' }),
    deleteUser: (id) => apiRequest(`/Users/${id}`, { method: 'DELETE' }),
};

export const watchHistoryService = {
    getByProfile: (profileId) => apiRequest(`/WatchHistory/profile/${profileId}`),
    getWatchHistory: (profileId) => apiRequest(`/WatchHistory/profile/${profileId}`),
    getContinueWatching: (profileId) => apiRequest(`/WatchHistory/continue-watching/profile/${profileId}`),

    getById: (id) => apiRequest(`/WatchHistory/${id}`),
    getByContentAndProfile: (contentId, profileId) =>
        apiRequest(`/WatchHistory/content/${contentId}/profile/${profileId}`),

    create: (payload) => apiRequest('/WatchHistory', { method: 'POST', body: payload }),
    update: (id, payload) => apiRequest(`/WatchHistory/${id}`, { method: 'PUT', body: payload }),
    delete: (id) => apiRequest(`/WatchHistory/${id}`, { method: 'DELETE' }),

    // Simple "progress update" endpoint your UI calls
    updateWatchProgress: (profileId, contentId, payload) => {
        // Convert the payload to match backend DTO requirements
        // Backend expects watchedDuration as TimeSpan string (e.g., "00:02:30")
        const formatTimeSpan = (seconds) => {
            const hours = Math.floor(seconds / 3600);
            const minutes = Math.floor((seconds % 3600) / 60);
            const secs = seconds % 60;
            return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
        };

        // Helper to convert duration strings like "2h 30m" to seconds
        const parseDurationToSeconds = (durationStr) => {
            if (typeof durationStr === 'number') return durationStr;
            if (!durationStr) return 0;
            
            let totalSeconds = 0;
            const hoursMatch = durationStr.match(/(\d+)h/);
            const minutesMatch = durationStr.match(/(\d+)m/);
            
            if (hoursMatch) totalSeconds += parseInt(hoursMatch[1]) * 3600;
            if (minutesMatch) totalSeconds += parseInt(minutesMatch[1]) * 60;
            
            return totalSeconds;
        };

        const backendPayload = {
            profileId: profileId,
            contentId: contentId,
            episodeId: payload.episodeId || null,
            watchedPercentage: payload.completed ? 100 : (payload.watchedPercentage || 0),
            watchedDuration: formatTimeSpan(parseDurationToSeconds(payload.watchedDuration || 0)),
            isCompleted: payload.completed || false
        };
        

        
        return apiRequest('/WatchHistory', {
            method: 'POST',
            body: backendPayload,
        });
    },

    // What WatchAgainPage expects - now shows all watched content
    getCompletedWatchHistory: async (profileId) => {
        try {
            // Get all watch history, not just completed
            const result = await apiRequest(`/WatchHistory/profile/${profileId}`);
            return result || [];
        } catch {
            return [];
        }
    },

    // Upsert helper (kept for compatibility)
    upsert: async (payload) => {
        const { profileId, contentId } = payload || {};
        if (!profileId || !contentId) throw new Error('upsert requires profileId and contentId');
        try {
            const existing = await apiRequest(`/WatchHistory/content/${contentId}/profile/${profileId}`);
            if (existing?.id) {
                return await apiRequest(`/WatchHistory/${existing.id}`, {
                    method: 'PUT',
                    body: { ...existing, ...payload },
                });
            }
            return await apiRequest('/WatchHistory', { method: 'POST', body: payload });
        } catch (err) {
            if (err.status === 404) {
                return await apiRequest('/WatchHistory', { method: 'POST', body: payload });
            }
            throw err;
        }
    },
};

export { apiRequest, API_BASE_URL };
