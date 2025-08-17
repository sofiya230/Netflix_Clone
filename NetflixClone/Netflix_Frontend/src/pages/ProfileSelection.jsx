import { useEffect, useState, useCallback } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { profileService } from '../../services/api';
import '../../styles/profile-selection.css';

export default function ProfileSelection() {
    const { currentUser, selectProfile } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const [profiles, setProfiles] = useState(currentUser?.profiles ?? []);
    const [loading, setLoading] = useState(false);

    useEffect(() => {

        

        const load = async () => {
            const userId = currentUser?.id || currentUser?.Id;
            if (!userId) {
                return;
            }
            
            setLoading(true);
            try {
                const data = await profileService.getProfilesByUserId(userId);
                setProfiles(Array.isArray(data) ? data : []);
            } catch (error) {
                setProfiles([]);
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [currentUser?.id, currentUser?.Id]);
    

    useEffect(() => {
        const userId = currentUser?.id || currentUser?.Id;
        if (userId && location.pathname === '/profiles') {


            const loadProfiles = async () => {
                if (!userId) return;
                setLoading(true);
                try {
                    const data = await profileService.getProfilesByUserId(userId);
                    setProfiles(Array.isArray(data) ? data : []);
                } catch (error) {
                    setProfiles([]);
                } finally {
                    setLoading(false);
                }
            };
            loadProfiles();
        }
            }, [location.pathname, currentUser?.id, currentUser?.Id]);

    const handleChoose = (p) => {

        selectProfile(p);
        navigate('/home');
    };

    const handleEdit = (p) => navigate(`/profiles/edit/${p.id}`);
    const handleDelete = async (p) => {
        if (!window.confirm(`Delete profile "${p.name}"?`)) return;
        try {
            await profileService.deleteProfile(p.id);
            setProfiles((prev) => prev.filter((x) => x.id !== p.id));
        } catch (e) {
        }
    };

    const handleAdd = () => navigate('/profiles/add');
    

    const refreshProfiles = useCallback(async () => {
        const userId = currentUser?.id || currentUser?.Id;
        if (!userId) {
            return;
        }
        
        setLoading(true);
        try {
            const data = await profileService.getProfilesByUserId(userId);
            setProfiles(Array.isArray(data) ? data : []);
        } catch (error) {
            setProfiles([]);
        } finally {
            setLoading(false);
        }
    }, [currentUser?.id, currentUser?.Id]);

    if (loading) {
        return (
            <div className="profile-selection-container">
                <p>Loading profiles…</p>
            </div>
        );
    }


    if (!currentUser) {
        return (
            <div className="profile-selection-container">
                <p>Please log in to view profiles.</p>
            </div>
        );
    }

    return (
        <div className="profile-selection-container">
            <h2 className="heading">Select a Profile</h2>
            


            <div className="profiles-grid">
                {profiles.map((p) => (
                    <div key={p.id} className="profile-box">
                        <button
                            className="profile-card"
                            onClick={() => handleChoose(p)}
                            aria-label={`Select profile ${p.name}`}
                        >
                            <img
                                className="profile-avatar"
                                src={p.avatarUrl || '/images/defaultavatar.jpg'}
                                alt=""
                                onError={(e) => { e.target.src = '/images/defaultavatar.jpg'; }}
                            />

                            <div className="profile-name">{p.name}</div>
                        </button>

                        <div className="profile-actions">
                            <button className="edit-btn" onClick={() => handleEdit(p)}>Edit</button>
                            <button className="delete-btn" onClick={() => handleDelete(p)}>Delete</button>
                        </div>
                    </div>
                ))}

                <button className="add-profile-box" onClick={handleAdd}>
                    <div className="plus">+</div>
                    <div>Add Profile</div>
                </button>
            </div>
        </div>
    );
}
